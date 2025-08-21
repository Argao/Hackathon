using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Infrastructure.Repositories;

public class SimulacaoRepository(AppDbContext context) : ISimulacaoRepository
{
    public async Task<Simulacao> AdicionarAsync(Simulacao simulacao, CancellationToken ct)
    {
        context.Simulacoes.Add(simulacao);
        await context.SaveChangesAsync(ct);
        return simulacao;
    }

    public async Task<(IEnumerable<Simulacao> Data, int TotalRecords)> ListarPaginadoAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = context.Simulacoes
            .Include(s => s.Resultados) // Carrega os resultados relacionados
            .AsQueryable();
        
        // Contagem total para paginação
        var totalRecords = await query.CountAsync(ct);
        
        // Ordenação para consistência na paginação
        query = query.OrderByDescending(s => s.DataReferencia)
            .ThenByDescending(s => s.IdSimulacao);

        // Aplicar paginação
        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (data, totalRecords);
    }

    public async Task<IEnumerable<VolumeSimuladoAgregado>> ObterVolumeSimuladoPorProdutoAsync(DateOnly dataReferencia, CancellationToken ct)
    {
        var resultado = await context.Simulacoes
            .Include(s => s.Resultados)
            .ThenInclude(r => r.Parcelas)
            .Where(s => s.DataReferencia == dataReferencia)
            .GroupBy(s => new { s.CodigoProduto, s.DescricaoProduto })
            .Select(g => new VolumeSimuladoAgregado
            {
                CodigoProduto = g.Key.CodigoProduto,
                DescricaoProduto = g.Key.DescricaoProduto,
                TaxaMediaJuro = g.Average(s => s.TaxaJuros),
                ValorMedioPrestacao = g.SelectMany(s => s.Resultados)
                    .SelectMany(r => r.Parcelas)
                    .Average(p => p.ValorPrestacao),
                ValorTotalDesejado = g.Sum(s => s.ValorDesejado),
                ValorTotalCredito = g.SelectMany(s => s.Resultados)
                    .Sum(r => r.ValorTotal)
            })
            .ToListAsync(ct);

        return resultado;
    }
}
