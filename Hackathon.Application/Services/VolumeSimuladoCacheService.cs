using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Services;

/// <summary>
/// Serviço de cache para volume simulado com estratégia híbrida
/// </summary>
public interface IVolumeSimuladoCacheService
{
    Task<IEnumerable<VolumeSimuladoProdutoDto>> GetVolumeSimuladoAsync(DateOnly dataReferencia, CancellationToken ct = default);
    void InvalidateCache(DateOnly dataReferencia);
}

public class VolumeSimuladoCacheService : IVolumeSimuladoCacheService
{
    private readonly IMemoryCache _cache;
    private readonly ISimulacaoRepository _repository;
    private readonly ILogger<VolumeSimuladoCacheService> _logger;
    
    public VolumeSimuladoCacheService(
        IMemoryCache cache,
        ISimulacaoRepository repository,
        ILogger<VolumeSimuladoCacheService> logger)
    {
        _cache = cache;
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<VolumeSimuladoProdutoDto>> GetVolumeSimuladoAsync(DateOnly dataReferencia, CancellationToken ct = default)
    {
        var cacheKey = $"volume_simulado_{dataReferencia:yyyy-MM-dd}";
        
        // ✅ OTIMIZAÇÃO: Verificar cache primeiro
        if (_cache.TryGetValue(cacheKey, out IEnumerable<VolumeSimuladoProdutoDto>? cachedData))
        {
            _logger.LogDebug("✅ Cache hit para volume simulado: {DataReferencia}", dataReferencia);
            return cachedData!;
        }

        // ✅ OTIMIZAÇÃO: Buscar dados do repositório
        _logger.LogDebug("❌ Cache miss para volume simulado: {DataReferencia}", dataReferencia);
        var dados = await _repository.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, ct);

        // ✅ OTIMIZAÇÃO: TTL dinâmico baseado na data
        var ttl = CalcularTTL(dataReferencia);
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl,
            Size = 1
        };

        _cache.Set(cacheKey, dados, cacheOptions);
        _logger.LogDebug("✅ Dados armazenados no cache com TTL: {TTL} minutos", ttl.TotalMinutes);

        return dados;
    }

    public void InvalidateCache(DateOnly dataReferencia)
    {
        var cacheKey = $"volume_simulado_{dataReferencia:yyyy-MM-dd}";
        _cache.Remove(cacheKey);
        _logger.LogDebug("🗑️ Cache invalidado para: {DataReferencia}", dataReferencia);
    }

    /// <summary>
    /// Calcula TTL baseado na data de referência
    /// </summary>
    private static TimeSpan CalcularTTL(DateOnly dataReferencia)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var diasDiff = dataReferencia.DayNumber - hoje.DayNumber;

        return diasDiff switch
        {
            0 => TimeSpan.FromMinutes(5),    // Hoje: 5 minutos
            -1 => TimeSpan.FromMinutes(15),  // Ontem: 15 minutos
            <= -7 and >= -30 => TimeSpan.FromMinutes(30), // Última semana: 30 minutos
            _ => TimeSpan.FromMinutes(60)    // Histórico: 60 minutos
        };
    }
}
