using Hackathon.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Hackathon.Application.Behaviors;

/// <summary>
/// Behavior para telemetria automática de performance
/// Cross-cutting concern aplicado a todos os handlers
/// SRP: Apenas coleta de métricas
/// </summary>
public class TelemetriaBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly ITelemetriaService _telemetria;
    private readonly ILogger<TelemetriaBehavior<TRequest, TResponse>> _logger;

    public TelemetriaBehavior(ITelemetriaService telemetria, ILogger<TelemetriaBehavior<TRequest, TResponse>> logger)
    {
        _telemetria = telemetria;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestName = typeof(TRequest).Name;
        
        try
        {
            _logger.LogInformation("🚀 Iniciando execução de {RequestName}", requestName);
            
            var response = await next();
            
            stopwatch.Stop();
            
            // Registrar métricas de sucesso
            await _telemetria.RegistrarMetricaAsync(
                requestName,
                requestName, 
                stopwatch.ElapsedMilliseconds,
                true,
                200,
                cancellationToken);
                
            _logger.LogInformation("✅ {RequestName} executado com sucesso em {ElapsedMs}ms", 
                requestName, stopwatch.ElapsedMilliseconds);
                
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Registrar métricas de erro
            await _telemetria.RegistrarMetricaAsync(
                requestName,
                requestName,
                stopwatch.ElapsedMilliseconds,
                false,
                500,
                cancellationToken);
                
            _logger.LogError(ex, "❌ Erro em {RequestName} após {ElapsedMs}ms", 
                requestName, stopwatch.ElapsedMilliseconds);
                
            throw;
        }
    }
}