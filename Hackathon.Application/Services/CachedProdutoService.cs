using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Services;

/// <summary>
/// Serviço simples de cache para produtos externos
/// </summary>
public interface ICachedProdutoService
{
    Task<Produto?> GetProdutoAdequadoAsync(ValorMonetario valor, PrazoMeses prazo, CancellationToken ct = default);
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

    public async Task<Produto?> GetProdutoAdequadoAsync(ValorMonetario valor, PrazoMeses prazo, CancellationToken ct = default)
    {
        var todosProdutos = await GetAllAsync(ct);
        
        return todosProdutos?.FirstOrDefault(p => p.AtendeRequisitos(valor, prazo.Meses));
    }

    public async Task<IEnumerable<Produto>?> GetAllAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CACHE_KEY_ALL, out List<Produto>? todosProdutos))
        {

            return todosProdutos;
        }


        
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

        }

        return todosProdutos;
    }

    public void InvalidateCache()
    {
        _cache.Remove(CACHE_KEY_ALL);

    }
}
