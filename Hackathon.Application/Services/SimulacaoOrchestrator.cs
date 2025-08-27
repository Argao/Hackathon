using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Results;
using Hackathon.Domain.Exceptions;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Services;

/// <summary>
/// Orquestrador com responsabilidade única: coordenar o fluxo de simulação
/// SRP: Apenas coordena, delega todas as operações específicas
/// </summary>
public class SimulacaoOrchestrator : ISimulacaoOrchestrator
{
    private readonly ICachedProdutoService _produtoService;
    private readonly ISimulacaoFactory _simulacaoFactory;
    private readonly ICalculadoraService _calculadoraService;
    private readonly ISimulacaoRepository _repository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<SimulacaoOrchestrator> _logger;
    private readonly IVolumeSimuladoCacheService _volumeCacheService;

    public SimulacaoOrchestrator(
        ICachedProdutoService produtoService,
        ISimulacaoFactory simulacaoFactory,
        ICalculadoraService calculadoraService,
        ISimulacaoRepository repository,
        IEventPublisher eventPublisher,
        IMapper mapper,
        ILogger<SimulacaoOrchestrator> logger,
        IVolumeSimuladoCacheService volumeCacheService)
    {
        _produtoService = produtoService;
        _simulacaoFactory = simulacaoFactory;
        _calculadoraService = calculadoraService;
        _repository = repository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _logger = logger;
        _volumeCacheService = volumeCacheService;
    }

    public async Task<SimulacaoResult> RealizarSimulacaoAsync(RealizarSimulacaoCommand command, CancellationToken cancellationToken)
    {
        // 1. Validar e converter (delegado para Command)
        var valueObjectsResult = command.ToValueObjects();
        if (!valueObjectsResult.IsSuccess)
            throw new ValidationException(valueObjectsResult.Error);

        var (valorEmprestimo, prazoMeses) = valueObjectsResult.Value;
        var valorMonetario = ValorMonetario.Create(valorEmprestimo.Valor).Value;

        // 2. Obter produto (delegado para serviço específico)
        var produto = await _produtoService.GetProdutoAdequadoAsync(valorMonetario, prazoMeses, cancellationToken);
        if (produto is null)
            throw new SimulacaoException($"Nenhum produto disponível para valor {valorEmprestimo} e prazo {prazoMeses}");

        // 3. Criar simulação (delegado para factory)
        var simulacao = _simulacaoFactory.CriarSimulacao(
            produto.Codigo,
            produto.Descricao,
            produto.TaxaMensal,
            valorMonetario,
            prazoMeses);

        // 4. Executar cálculos (delegado para service específico)
        var resultados = _calculadoraService.ExecutarCalculos(valorMonetario, produto.TaxaMensal, prazoMeses);
        simulacao.AdicionarResultados(resultados);

        // 5. Mapear resultado usando abstração genérica (SOLID + Clean Architecture)
        var result = _mapper.Map<Domain.Entities.Simulacao, SimulacaoResult>(simulacao);

        // 6. Persistir (crítico - aguarda)
        await _repository.AdicionarAsync(simulacao, cancellationToken);


        // ✅ OTIMIZAÇÃO: Invalidar cache de volume simulado
        _volumeCacheService.InvalidateCache(simulacao.DataReferencia);
        

        // 7. Publicar evento (não crítico - fire and forget)
        _eventPublisher.PublishAsync(result);

        return result;
    }
}