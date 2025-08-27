using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.ValueObjects;
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

    public async Task<IEnumerable<VolumeSimuladoProdutoDto>> ObterVolumeSimuladoPorProdutoAsync(DateOnly dataReferencia, CancellationToken ct)
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
            .Select(g => new VolumeSimuladoProdutoDto(
                CodigoProduto: g.Key.CodigoProduto,
                DescricaoProduto: g.Key.DescricaoProduto,
                TaxaMediaJuro: (decimal)g.Average(s => (double)s.TaxaJuros.Taxa),
                ValorMedioPrestacao: g.SelectMany(s => s.Resultados)
                    .SelectMany(r => r.Parcelas)
                    .Any() ? (decimal)g.SelectMany(s => s.Resultados)
                            .SelectMany(r => r.Parcelas)
                            .Average(p => (double)p.ValorPrestacao.Valor) : 0m,
                ValorTotalDesejado: g.Sum(s => s.ValorDesejado.Valor),
                ValorTotalCredito: g.SelectMany(s => s.Resultados)
                    .Sum(r => r.ValorTotal.Valor)
            ))
            .ToList();

        return resultado;
    }

    public async Task<int> ObterTotalSimulacoesAsync(CancellationToken ct)
    {
        return await context.Simulacoes.CountAsync(ct);
    }

    // OTIMIZAÇÃO: Método com projeção específica - evita carregar parcelas desnecessárias
    public async Task<IEnumerable<SimulacaoResumoDto>> ListarSimulacoesOtimizadoAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        // Buscar dados do banco primeiro
        var simulacoes = await context.Simulacoes
            .Include(s => s.Resultados) // Apenas resultados, sem parcelas
            .OrderByDescending(s => s.IdSimulacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

        // Fazer a projeção no cliente
        var resultado = simulacoes.Select(s => new SimulacaoResumoDto(
            s.IdSimulacao,
            s.ValorDesejado.Valor,
            s.PrazoMeses,
            s.Resultados.Sum(r => r.ValorTotal.Valor) // Usa ValorTotal já calculado
        ));

        return resultado;
    }
}
