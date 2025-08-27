using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Results;
using Hackathon.Domain.Exceptions;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

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
    private readonly IMemoryCache _cache;
    private readonly IVolumeSimuladoCacheService _volumeCacheService;

    // ✅ OTIMIZAÇÃO: Cache de resultados de simulação
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public SimulacaoOrchestrator(
        ICachedProdutoService produtoService,
        ISimulacaoFactory simulacaoFactory,
        ICalculadoraService calculadoraService,
        ISimulacaoRepository repository,
        IEventPublisher eventPublisher,
        IMapper mapper,
        ILogger<SimulacaoOrchestrator> logger,
        IMemoryCache cache,
        IVolumeSimuladoCacheService volumeCacheService)
    {
        _produtoService = produtoService;
        _simulacaoFactory = simulacaoFactory;
        _calculadoraService = calculadoraService;
        _repository = repository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
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

        // ✅ OTIMIZAÇÃO: Verificar cache antes de calcular
        var cacheKey = GerarCacheKey(valorMonetario, prazoMeses);
        if (_cache.TryGetValue(cacheKey, out SimulacaoResult cachedResult))
        {
            _logger.LogDebug("✅ Cache hit para simulação: {CacheKey}", cacheKey);
            return cachedResult;
        }

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

        // ✅ OTIMIZAÇÃO: Armazenar no cache
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration,
            Size = 1 // Tamanho estimado para o item do cache
        };
        _cache.Set(cacheKey, result, cacheOptions);
        _logger.LogDebug("✅ Cache miss - calculado e armazenado: {CacheKey}", cacheKey);

        // 6. Persistir (crítico - aguarda)
        await _repository.AdicionarAsync(simulacao, cancellationToken);
        _logger.LogInformation("✅ Simulação persistida - ID: {SimulacaoId}", result.Id);

        // ✅ OTIMIZAÇÃO: Invalidar cache de volume simulado
        _volumeCacheService.InvalidateCache(simulacao.DataReferencia);
        _logger.LogDebug("🗑️ Cache de volume simulado invalidado para: {Data}", simulacao.DataReferencia);

        // 7. Publicar evento (não crítico - fire and forget)
        _eventPublisher.PublishAsync(result);

        return result;
    }

    /// <summary>
    /// Gera chave única para cache baseada nos parâmetros da simulação
    /// </summary>
    private static string GerarCacheKey(ValorMonetario valor, PrazoMeses prazo)
    {
        return $"simulacao_{valor.Valor:F2}_{prazo.Meses}";
    }
}