using Hackathon.Application.DTOs.Responses;
using Hackathon.Application.Interfaces;
using Hackathon.Domain.Exceptions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Services;

/// <summary>
/// Serviço de telemetria com implementação fire-and-forget otimizada
/// </summary>
public class TelemetriaService : ITelemetriaService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TelemetriaService> _logger;

    public TelemetriaService(
        IServiceScopeFactory scopeFactory,
        ILogger<TelemetriaService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Registra métrica de forma fire-and-forget usando Task.Run para não bloquear o thread principal
    /// </summary>
    public async Task RegistrarMetricaAsync(
        string nomeApi, 
        string endpoint, 
        long tempoResposta, 
        bool sucesso, 
        int statusCode, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nomeApi) || string.IsNullOrWhiteSpace(endpoint))
        {
            _logger.LogWarning("Tentativa de registrar métrica com parâmetros inválidos: " +
                "NomeApi='{NomeApi}', Endpoint='{Endpoint}'", nomeApi, endpoint);
            return;
        }

        // Fire-and-forget: executa em background thread com scope próprio
        _ = Task.Run(async () =>
        {
            try
            {
                // Criar scope independente para evitar ObjectDisposedException
                using var scope = _scopeFactory.CreateScope();
                var metricaRepository = scope.ServiceProvider.GetRequiredService<IMetricaRepository>();

                var metrica = new MetricaRequisicao
                {
                    NomeApi = nomeApi.Trim(),
                    Endpoint = endpoint.Trim(),
                    TempoRespostaMs = tempoResposta,
                    Sucesso = sucesso,
                    StatusCode = statusCode,
                    DataHora = DateTime.UtcNow,
                };

                await metricaRepository.SalvarMetricaAsync(metrica, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico no serviço de telemetria ao registrar métrica");
            }
        }, cancellationToken);

        _logger.LogTrace("Métrica enfileirada: {NomeApi} - {Endpoint} - {TempoMs}ms", 
            nomeApi, endpoint, tempoResposta);
    }

    /// <summary>
    /// Obtém telemetria agregada por data com tratamento de erros
    /// </summary>
    public async Task<TelemetriaFinalResponseDTO> ObterTelemetriaPorDataAsync(
        DateOnly dataReferencia, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Consultando telemetria para data: {DataReferencia}", dataReferencia);

            using var scope = _scopeFactory.CreateScope();
            var metricaRepository = scope.ServiceProvider.GetRequiredService<IMetricaRepository>();
            
            var metricasAgregadas = await metricaRepository.ObterMetricasPorDataAsync(dataReferencia, cancellationToken);

            // Agrupar por NomeApi e calcular métricas agregadas
            var telemetriasPorApi = metricasAgregadas
                .GroupBy(m => m.NomeApi)
                .Select(grupo => new TelemetriaApiDTO(
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

            var response = new TelemetriaFinalResponseDTO(
                DataReferencia: dataReferencia,
                ListaEndpoints: telemetriasPorApi
            );

            _logger.LogInformation("Telemetria consultada com sucesso: {QtdApis} APIs, {TotalRequisicoes} requisições",
                telemetriasPorApi.Count, telemetriasPorApi.Sum(x => x.QtdRequisicoes));

            return response;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Consulta de telemetria cancelada para data: {DataReferencia}", dataReferencia);
            throw new SimulacaoException("Operação cancelada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar telemetria para data: {DataReferencia}", dataReferencia);
            throw new SimulacaoException($"Erro interno: {ex.Message}", ex);
        }
    }
}
