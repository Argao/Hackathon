using System.Diagnostics;
using Hackathon.Application.Interfaces;
using Microsoft.Extensions.Primitives;
using System.Threading.Channels;

namespace Hackathon.API.Middleware;

/// <summary>
/// Middleware para coleta de métricas de telemetria
/// Processamento em lote para minimizar overhead
/// </summary>
public class TelemetriaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TelemetriaMiddleware> _logger;
    private readonly Channel<MetricaData> _metricChannel;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IServiceProvider _serviceProvider;

    // Endpoints que devem ser ignorados na telemetria (para evitar overhead desnecessário)
    private static readonly HashSet<string> EndpointsIgnorados = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health",
        "/healthz", 
        "/ready",
        "/live",
        "/swagger",
        "/favicon.ico",
        "/robots.txt"
    };

    public TelemetriaMiddleware(
        RequestDelegate next,
        ILogger<TelemetriaMiddleware> logger,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
        
        // Configurar channel para processamento em lote
        _metricChannel = Channel.CreateUnbounded<MetricaData>(new UnboundedChannelOptions 
        { 
            SingleReader = true,
            SingleWriter = false 
        });
        _cancellationTokenSource = new CancellationTokenSource();
        
        // Iniciar worker em background
        _ = ProcessMetricsAsync(_cancellationTokenSource.Token);
    }

    /// <summary>
    /// Intercepta requisições HTTP para coletar métricas
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Verificar se deve ignorar este endpoint
        if (DeveIgnorarEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Iniciar cronômetro com alta precisão
        var stopwatch = Stopwatch.StartNew();
        
        // Capturar dados da requisição ANTES de processar
        var nomeApi = ExtrairNomeApi(context);
        var endpoint = $"{context.Request.Method} {context.Request.Path}";
        
        try
        {
            // Processar requisição normalmente
            await _next(context);
        }
        finally
        {
            // Parar cronômetro imediatamente após processamento
            stopwatch.Stop();

            // Processamento em lote: enfileirar métrica sem bloquear
            var metricaData = new MetricaData
            {
                NomeApi = nomeApi,
                Endpoint = endpoint,
                TempoResposta = stopwatch.ElapsedMilliseconds,
                Sucesso = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300,
                StatusCode = context.Response.StatusCode,
                DataHora = DateTime.UtcNow
            };

            // Fire-and-forget: enfileirar sem aguardar
            _ = _metricChannel.Writer.TryWrite(metricaData);
        }
    }

    /// <summary>
    /// Processa métricas em lote
    /// </summary>
    private async Task ProcessMetricsAsync(CancellationToken cancellationToken)
    {
        var batch = new List<MetricaData>(100);
        var batchTimeout = TimeSpan.FromSeconds(5);
        var lastProcessTime = DateTime.UtcNow;

        try
        {
            await foreach (var metric in _metricChannel.Reader.ReadAllAsync(cancellationToken))
            {
                batch.Add(metric);
                
                // Processar lote quando atingir 100 métricas ou após 5 segundos
                var shouldProcess = batch.Count >= 100 || 
                                  (DateTime.UtcNow - lastProcessTime) >= batchTimeout;
                
                if (shouldProcess && batch.Count > 0)
                {
                    await ProcessBatchAsync(batch);
                    batch.Clear();
                    lastProcessTime = DateTime.UtcNow;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Processar lote final antes de encerrar
            if (batch.Count > 0)
            {
                await ProcessBatchAsync(batch);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico no processamento de métricas em lote");
        }
    }

    /// <summary>
    /// Processa um lote de métricas de uma vez
    /// </summary>
    private async Task ProcessBatchAsync(List<MetricaData> batch)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var telemetriaService = scope.ServiceProvider.GetRequiredService<ITelemetriaService>();
            
            // Processar todas as métricas do lote
            foreach (var metric in batch)
            {
                await telemetriaService.RegistrarMetricaAsync(
                    metric.NomeApi,
                    metric.Endpoint,
                    metric.TempoResposta,
                    metric.Sucesso,
                    metric.StatusCode,
                    CancellationToken.None);
            }
            

        }
        catch (Exception ex)
        {

        }
    }

    private static string ExtrairNomeApi(HttpContext context)
    {
        // Tentar extrair do roteamento primeiro
        if (context.GetRouteData()?.Values.TryGetValue("controller", out var controller) == true)
        {
            return controller?.ToString() ?? "Unknown";
        }

        // Fallback: extrair do path
        var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments?.Length > 0)
        {
            return segments[0]; // Primeiro segmento após a raiz
        }

        return "Unknown";
    }
    
    private static bool DeveIgnorarEndpoint(PathString path)
    {
        var pathValue = path.Value;
        if (string.IsNullOrWhiteSpace(pathValue))
            return true;

        return EndpointsIgnorados.Any(ignored => 
            pathValue.StartsWith(ignored, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Dados da métrica para processamento em lote
    /// </summary>
    private class MetricaData
    {
        public string NomeApi { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public long TempoResposta { get; set; }
        public bool Sucesso { get; set; }
        public int StatusCode { get; set; }
        public DateTime DataHora { get; set; }
    }
}
