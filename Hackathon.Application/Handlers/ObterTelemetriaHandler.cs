using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Exceptions;
using Hackathon.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Handlers;

/// <summary>
/// Handler simplificado - acessa diretamente o repositório
/// </summary>
public class ObterTelemetriaHandler : IRequestHandler<ObterTelemetriaQuery, TelemetriaResult>
{
    private readonly IMetricaRepository _metricaRepository;
    private readonly ILogger<ObterTelemetriaHandler> _logger;

    public ObterTelemetriaHandler(IMetricaRepository metricaRepository, ILogger<ObterTelemetriaHandler> logger)
    {
        _metricaRepository = metricaRepository;
        _logger = logger;
    }

    public async Task<TelemetriaResult> Handle(ObterTelemetriaQuery request, CancellationToken cancellationToken)
    {
        var dataReferencia = request.GetValidDataReferencia();
        var metricasAgregadas = await _metricaRepository.ObterMetricasPorDataAsync(dataReferencia, cancellationToken);
        
        if (!metricasAgregadas.Any())
        {

            throw new SimulacaoException($"Nenhum dado de telemetria encontrado para a data {dataReferencia:yyyy-MM-dd}");
        }

        var telemetriasPorApi = metricasAgregadas
            .GroupBy(m => m.NomeApi)
            .Select(grupo => new TelemetriaApiResult(
                grupo.Key,
                grupo.Sum(x => x.QtdRequisicoes),
                Math.Round(grupo.Average(x => x.TempoMedio), 0),
                grupo.Min(x => x.TempoMinimo),
                grupo.Max(x => x.TempoMaximo),
                Math.Round(
                    grupo.Sum(x => x.QtdRequisicoes * x.PercentualSucesso) / grupo.Sum(x => x.QtdRequisicoes), 
                    2)
            ))
            .OrderBy(t => t.NomeApi)
            .ToList();

        return new TelemetriaResult(dataReferencia, telemetriasPorApi);
    }
}
