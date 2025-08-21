using FluentValidation;
using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;
using Mapster;

namespace Hackathon.Application.Services;

/// <summary>
/// Serviço de aplicação para operações de simulação
/// </summary>
public class SimulacaoService : ISimulacaoService
{
    private readonly ISimulacaoRepository _simulacaoRepository;
    private readonly ICachedProdutoService _cachedProdutoService;
    private readonly IEnumerable<ICalculadoraAmortizacao> _calculadoras;
    private readonly IValidator<RealizarSimulacaoCommand> _simulacaoValidator;
    private readonly IValidator<ListarSimulacoesQuery> _listarValidator;
    private readonly IValidator<ObterVolumeSimuladoQuery> _volumeValidator;
    private readonly IEventHubService _eventHubService;

    public SimulacaoService(
        ICachedProdutoService cachedProdutoService,
        ISimulacaoRepository simulacaoRepository, 
        IEnumerable<ICalculadoraAmortizacao> calculadoras,
        IValidator<RealizarSimulacaoCommand> simulacaoValidator,
        IValidator<ListarSimulacoesQuery> listarValidator,
        IValidator<ObterVolumeSimuladoQuery> volumeValidator,
        IEventHubService eventHubService)
    {
        _cachedProdutoService = cachedProdutoService;
        _simulacaoRepository = simulacaoRepository;
        _calculadoras = calculadoras;
        _simulacaoValidator = simulacaoValidator;
        _listarValidator = listarValidator;
        _volumeValidator = volumeValidator;
        _eventHubService = eventHubService;
    }

    /// <summary>
    /// Executa uma simulação de crédito
    /// </summary>
    public async Task<Result<SimulacaoResult>> RealizarSimulacaoAsync(RealizarSimulacaoCommand command, CancellationToken ct)
    {
        // Validação do comando
        var validationResult = await _simulacaoValidator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
            return Result<SimulacaoResult>.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

        // Validação com Value Objects
        var valueObjectsResult = command.ToValueObjects();
        if (!valueObjectsResult.IsSuccess)
            return Result<SimulacaoResult>.Failure(valueObjectsResult.Error);

        var (valorEmprestimo, prazoMeses) = valueObjectsResult.Value;

        // Buscar produto adequado usando cache
        var produto = await _cachedProdutoService.GetProdutoAdequadoAsync(valorEmprestimo, prazoMeses, ct);
        if (produto is null)
            return Result<SimulacaoResult>.Failure(
                $"Nenhum produto disponível para valor {valorEmprestimo} e prazo {prazoMeses}");

        // Criar simulação simples
        var simulacao = (command, produto).Adapt<Simulacao>();

        // Calcular amortizações
        var resultados = _calculadoras
            .Select(c => c.Calcular(valorEmprestimo, produto.TaxaMensal, prazoMeses))
            .ToList();
        
        simulacao.Resultados = resultados;

        // Mapear resultado direto dos cálculos antes de persistir
        var result = new SimulacaoResult(
            Id: simulacao.IdSimulacao,
            CodigoProduto: simulacao.CodigoProduto,
            DescricaoProduto: simulacao.DescricaoProduto,
            TaxaJuros: simulacao.TaxaJuros,
            Resultados: resultados.Select(r => new ResultadoCalculoAmortizacao(
                TipoAmortizacao: r.Tipo.ToString(),
                Parcelas: r.Parcelas.Select(p => new ParcelaCalculada(
                    Numero: p.Numero,
                    ValorAmortizacao: p.ValorAmortizacao,
                    ValorJuros: p.ValorJuros,
                    ValorPrestacao: p.ValorPrestacao
                )).ToList()
            )).ToList()
        );

        // Executar persistência e envio ao EventHub em paralelo para máxima performance
        var persistirTask = _simulacaoRepository.AdicionarAsync(simulacao, ct);
        var eventhubTask = _eventHubService.EnviarSimulacaoAsync(result, ct);

        try
        {
            // Aguarda ambas as operações em paralelo
            await Task.WhenAll(persistirTask, eventhubTask);
        }
        catch (Exception)
        {
            // Se o EventHub falhar, mas a persistência for bem-sucedida, 
            // ainda retornamos sucesso pois o requisito principal foi atendido
            // O log de erro já foi registrado no EventHubService
            await persistirTask; // Garante que a persistência foi concluída
        }

        return Result<SimulacaoResult>.Success(result);
    }

    /// <summary>
    /// Lista simulações de forma paginada
    /// </summary>
    public async Task<Result<PagedResult<SimulacaoResumoResult>>> ListarSimulacoesAsync(ListarSimulacoesQuery query, CancellationToken ct)
    {
        // Validação da query
        var validationResult = await _listarValidator.ValidateAsync(query, ct);
        if (!validationResult.IsValid)
            return Result<PagedResult<SimulacaoResumoResult>>.Failure(
                string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var pageNumber = query.GetValidPageNumber();
        var pageSize = query.GetValidPageSize();

        var totalItems = await _simulacaoRepository.ObterTotalSimulacoesAsync(ct);
        var simulacoes = await _simulacaoRepository.ListarSimulacoesAsync(pageNumber, pageSize, ct);

        var resumos = simulacoes.Select(s => new SimulacaoResumoResult(
            Id: s.IdSimulacao,
            ValorDesejado: s.ValorDesejado,
            Prazo: s.PrazoMeses,
            ValorTotalParcelas: s.Resultados.SelectMany(r => r.Parcelas).Sum(p => p.ValorPrestacao)
        )).ToList();

        var result = new PagedResult<SimulacaoResumoResult>(
            Items: resumos,
            TotalItems: totalItems,
            CurrentPage: pageNumber,
            PageSize: pageSize
        );

        return Result<PagedResult<SimulacaoResumoResult>>.Success(result);
    }

    /// <summary>
    /// Obtém volume simulado por data
    /// </summary>
    public async Task<Result<VolumeSimuladoResult>> ObterVolumeSimuladoAsync(ObterVolumeSimuladoQuery query, CancellationToken ct)
    {
        // Validação da query
        var validationResult = await _volumeValidator.ValidateAsync(query, ct);
        if (!validationResult.IsValid)
            return Result<VolumeSimuladoResult>.Failure(
                string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var dadosAgregados = await _simulacaoRepository.ObterVolumeSimuladoPorProdutoAsync(query.DataReferencia, ct);
        
        var produtos = dadosAgregados.Adapt<List<VolumeSimuladoProdutoResult>>();
        
        var result = new VolumeSimuladoResult(
            DataReferencia: query.DataReferencia,
            Produtos: produtos
        );

        return Result<VolumeSimuladoResult>.Success(result);
    }
}