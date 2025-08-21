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
        // Primeiro, buscar as simulações do banco
        var simulacoes = await context.Simulacoes
            .Include(s => s.Resultados)
            .ThenInclude(r => r.Parcelas)
            .Where(s => s.DataReferencia == dataReferencia)
            .ToListAsync(ct);

        // Fazer as operações de agregação no cliente (C#)
        var resultado = simulacoes
            .GroupBy(s => new { s.CodigoProduto, s.DescricaoProduto })
            .Select(g => new VolumeSimuladoAgregado
            {
                CodigoProduto = g.Key.CodigoProduto,
                DescricaoProduto = g.Key.DescricaoProduto,
                TaxaMediaJuro = (decimal)g.Average(s => (double)s.TaxaJuros),
                ValorMedioPrestacao = g.SelectMany(s => s.Resultados)
                    .SelectMany(r => r.Parcelas)
                    .Any() ? (decimal)g.SelectMany(s => s.Resultados)
                            .SelectMany(r => r.Parcelas)
                            .Average(p => (double)p.ValorPrestacao) : 0,
                ValorTotalDesejado = g.Sum(s => s.ValorDesejado),
                ValorTotalCredito = g.SelectMany(s => s.Resultados)
                    .Sum(r => r.ValorTotal)
            })
            .ToList();

        return resultado;
    }

    public async Task<int> ObterTotalSimulacoesAsync(CancellationToken ct)
    {
        return await context.Simulacoes.CountAsync(ct);
    }

    public async Task<IEnumerable<Simulacao>> ListarSimulacoesAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        return await context.Simulacoes
            .Include(s => s.Resultados)
                .ThenInclude(r => r.Parcelas)
            .OrderByDescending(s => s.DataReferencia)
                .ThenByDescending(s => s.IdSimulacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }
}
