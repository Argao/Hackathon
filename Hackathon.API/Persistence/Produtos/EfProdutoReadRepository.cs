using Hackathon.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.API.Persistence.Produtos;

public class EfProdutoReadRepository(ProdutosDbContext context) : IProdutoReadRepository
{
    public async Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken ct)
    {
        var itens = await context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Codigo)
            .ToListAsync(ct);

        return itens;
    }

    public async Task<Produto?> SelecionarProdutoParaAsync(decimal valor, int prazo, CancellationToken ct)
    {
        return
            await context.Produtos.AsNoTracking()
                .Where(p => prazo >= p.MinMeses
                            && (p.MaxMeses == null || prazo <= p.MaxMeses)
                            && valor >= p.MinValor
                            && (p.MaxValor == null || valor <= p.MaxValor))
                .OrderBy(p => p.Codigo)
                .FirstOrDefaultAsync(ct);
    }
}