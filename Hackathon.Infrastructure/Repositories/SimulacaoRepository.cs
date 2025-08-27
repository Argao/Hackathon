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
        // Configurar para inserção em lote
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        
        context.Simulacoes.Add(simulacao);
        
        // ✅ OTIMIZAÇÃO: Usar SaveChangesAsync com configuração otimizada
        await context.SaveChangesAsync(ct);
        
        context.ChangeTracker.AutoDetectChangesEnabled = true;
        return simulacao;
    }

    public async Task<IEnumerable<VolumeSimuladoProdutoDto>> ObterVolumeSimuladoPorProdutoAsync(DateOnly dataReferencia, CancellationToken ct)
    {
        // ✅ OTIMIZAÇÃO: Consulta em duas etapas para melhor performance
        var simulacoes = await context.Simulacoes
            .Include(s => s.Resultados)
            .Where(s => s.DataReferencia == dataReferencia)
            .AsNoTracking()
            .ToListAsync(ct);

        var resultado = simulacoes
            .GroupBy(s => new { s.CodigoProduto, s.DescricaoProduto })
            .Select(g => new VolumeSimuladoProdutoDto(
                g.Key.CodigoProduto,
                g.Key.DescricaoProduto,
                g.Average(s => s.TaxaJuros.Taxa),
                g.SelectMany(s => s.Resultados).Average(r => r.ValorTotal.Valor),
                g.Sum(s => s.ValorDesejado.Valor),
                g.SelectMany(s => s.Resultados).Sum(r => r.ValorTotal.Valor)
            ))
            .ToList();

        return resultado;
    }

    public async Task<int> ObterTotalSimulacoesAsync(CancellationToken ct)
    {
        return await context.Simulacoes.CountAsync(ct);
    }

    // ✅ OTIMIZAÇÃO: Projeção direta sem includes desnecessários
    public async Task<IEnumerable<SimulacaoResumoDto>> ListarSimulacoesOtimizadoAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var simulacoes = await context.Simulacoes
            .Include(s => s.Resultados)
            .OrderByDescending(s => s.IdSimulacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

        var resultado = simulacoes.Select(s => new SimulacaoResumoDto(
            s.IdSimulacao,
            s.ValorDesejado.Valor,
            s.PrazoMeses.Meses,
            s.Resultados.Select(r => new ValorTotalAmortizacaoDto(
                r.Tipo.ToString(),
                r.ValorTotal.Valor
            )).ToList()
        ));

        return resultado;
    }
}
