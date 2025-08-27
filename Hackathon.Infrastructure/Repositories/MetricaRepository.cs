using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Repositories;

public class MetricaRepository : IMetricaRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<MetricaRepository> _logger;

    public MetricaRepository(AppDbContext context, ILogger<MetricaRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SalvarMetricaAsync(MetricaRequisicao metrica, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Metricas.AddAsync(metrica, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            

        }
        catch (OperationCanceledException)
        {
            // Cancelamento é esperado durante shutdown - não loggar como erro

        }
        catch (Exception ex)
        {
            // Log do erro mas NÃO propaga exceção (fire-and-forget)
            _logger.LogError(ex, "Erro ao salvar métrica");
        }
    }

    public async Task<List<MetricaAgregada>> ObterMetricasPorDataAsync(
        DateOnly dataReferencia, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var dataInicio = dataReferencia.ToDateTime(TimeOnly.MinValue);
            var dataFim = dataReferencia.ToDateTime(TimeOnly.MaxValue);

            var metricas = await _context.Metricas
                .Where(m => m.DataHora >= dataInicio && m.DataHora <= dataFim)
                .GroupBy(m => new { m.NomeApi, m.Endpoint })
                .Select(g => new MetricaAgregada
                {
                    NomeApi = g.Key.NomeApi,
                    Endpoint = g.Key.Endpoint,
                    QtdRequisicoes = g.Count(),
                    TempoMedio = Math.Round(g.Average(x => x.TempoRespostaMs), 2),
                    TempoMinimo = g.Min(x => x.TempoRespostaMs),
                    TempoMaximo = g.Max(x => x.TempoRespostaMs),
                    PercentualSucesso = Math.Round((double)g.Count(x => x.Sucesso) / g.Count(), 4)
                })
                .OrderBy(m => m.NomeApi)
                .ThenBy(m => m.Endpoint)
                .ToListAsync(cancellationToken);

            return metricas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar métricas para data {DataReferencia}", dataReferencia);
            return new List<MetricaAgregada>(); // Retorna lista vazia em caso de erro
        }
    }
}
