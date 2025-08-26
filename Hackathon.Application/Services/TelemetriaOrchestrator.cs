using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Exceptions;
using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Services;

/// <summary>
/// Orquestrador com responsabilidade única: coordenar o fluxo de telemetria
/// SRP: Apenas coordena, delega todas as operações específicas
/// </summary>
public class TelemetriaOrchestrator : ITelemetriaOrchestrator
{
    private readonly IMetricaRepository _metricaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TelemetriaOrchestrator> _logger;

    public TelemetriaOrchestrator(
        IMetricaRepository metricaRepository,
        IMapper mapper,
        ILogger<TelemetriaOrchestrator> logger)
    {
        _metricaRepository = metricaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TelemetriaResult> ObterTelemetriaAsync(ObterTelemetriaQuery query, CancellationToken cancellationToken)
    {
        // 1. Validar data de referência (delegado para Query)
        var dataReferencia = query.GetValidDataReferencia();

        // 2. Obter dados de telemetria diretamente do repositório
        var metricasAgregadas = await _metricaRepository.ObterMetricasPorDataAsync(dataReferencia, cancellationToken);
        
        if (!metricasAgregadas.Any())
        {
            _logger.LogInformation("Nenhum dado de telemetria encontrado para data: {DataReferencia}", dataReferencia);
            throw new SimulacaoException($"Nenhum dado de telemetria encontrado para a data {dataReferencia:yyyy-MM-dd}");
        }

        // 3. Agrupar por NomeApi e calcular métricas agregadas
        var telemetriasPorApi = metricasAgregadas
            .GroupBy(m => m.NomeApi)
            .Select(grupo => new Application.DTOs.Responses.TelemetriaApiDTO(
                NomeApi: grupo.Key,
                QtdRequisicoes: grupo.Sum(x => x.QtdRequisicoes),
                TempoMedio: Math.Round(grupo.Average(x => x.TempoMedio), 0),
                TempoMinimo: grupo.Min(x => x.TempoMinimo),
                TempoMaximo: grupo.Max(x => x.TempoMaximo),
                PercentualSucesso: Math.Round(
                    grupo.Sum(x => x.QtdRequisicoes * x.PercentualSucesso) / grupo.Sum(x => x.QtdRequisicoes), 
                    2)
            ))
            .OrderBy(t => t.NomeApi)
            .ToList();

        var telemetriaDTO = new Application.DTOs.Responses.TelemetriaFinalResponseDTO(
            DataReferencia: dataReferencia,
            ListaEndpoints: telemetriasPorApi
        );

        // 4. Mapear resultado usando abstração genérica (SOLID + Clean Architecture)
        var result = _mapper.Map<Application.DTOs.Responses.TelemetriaFinalResponseDTO, TelemetriaResult>(telemetriaDTO);

        _logger.LogInformation("✅ Telemetria consultada com sucesso - Data: {DataReferencia}, APIs: {QtdApis}", 
            dataReferencia, result.ListaEndpoints.Count);

        return result;
    }
}
