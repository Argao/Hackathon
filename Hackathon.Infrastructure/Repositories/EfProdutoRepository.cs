using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Repositories;

/// <summary>
/// Repositório para produtos usando Entity Framework (SQL Server externo)
/// </summary>
public class EfProdutoRepository : IProdutoRepository
{
    private readonly ProdutoDbContext _context;
    private readonly ILogger<EfProdutoRepository> _logger;

    public EfProdutoRepository(ProdutoDbContext context, ILogger<EfProdutoRepository> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<IEnumerable<Produto>?> GetAllAsync(CancellationToken ct = default)
    {
        
        // PERFORMANCE: AsNoTracking pois são dados read-only
        return await _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Codigo)
            .ToListAsync(ct);
    }
}
