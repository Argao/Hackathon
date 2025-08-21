using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Services;

/// <summary>
/// Serviço simples de cache para produtos externos
/// </summary>
public interface ICachedProdutoService
{
    Task<Produto?> GetProdutoAdequadoAsync(decimal valor, int prazo, CancellationToken ct = default);
    Task<IEnumerable<Produto>?> GetAllAsync(CancellationToken ct = default);
    void InvalidateCache();
}

public class CachedProdutoService : ICachedProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedProdutoService> _logger;
    
    private const int CACHE_DURATION_MINUTES = 5;
    private const string CACHE_KEY_ALL = "produtos_all";

    public CachedProdutoService(
        IProdutoRepository produtoRepository,
        IMemoryCache cache,
        ILogger<CachedProdutoService> logger)
    {
        _produtoRepository = produtoRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Produto?> GetProdutoAdequadoAsync(decimal valor, int prazo, CancellationToken ct = default)
    {
        // Buscar todos os produtos em cache
        var todosProdutos = await GetAllAsync(ct);
        
        // Aplicar filtro usando dados em memória
        return todosProdutos?.FirstOrDefault(p => p.AtendeRequisitos(valor, prazo));
    }

    public async Task<IEnumerable<Produto>?> GetAllAsync(CancellationToken ct = default)
    {
        // Tentar buscar do cache primeiro
        if (_cache.TryGetValue(CACHE_KEY_ALL, out List<Produto>? todosProdutos))
        {
            _logger.LogDebug("Produtos obtidos do cache ({Count} produtos)", todosProdutos?.Count ?? 0);
            return todosProdutos;
        }

        _logger.LogDebug("Cache de produtos não encontrado, consultando banco");
        
        // Buscar do banco
        todosProdutos = (await _produtoRepository.GetAllAsync(ct))?.ToList();
        
        if (todosProdutos?.Any() == true)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES),
                Priority = CacheItemPriority.High,
                Size = 1
            };
            
            _cache.Set(CACHE_KEY_ALL, todosProdutos, cacheOptions);
            _logger.LogDebug("Lista de produtos adicionada ao cache ({Count} produtos)", todosProdutos.Count);
        }

        return todosProdutos;
    }

    public void InvalidateCache()
    {
        _cache.Remove(CACHE_KEY_ALL);
        _logger.LogInformation("Cache de produtos invalidado");
    }
}
