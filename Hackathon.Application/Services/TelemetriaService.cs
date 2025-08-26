using Hackathon.Application.Interfaces;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
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


}
