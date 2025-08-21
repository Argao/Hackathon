using System.Diagnostics;
using Hackathon.Application.Interfaces;
using Microsoft.Extensions.Primitives;

namespace Hackathon.API.Middleware;

/// <summary>
/// Middleware para coleta automática de métricas de telemetria
/// Implementa fire-and-forget para não impactar performance da API
/// </summary>
public class TelemetriaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TelemetriaMiddleware> _logger;

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
        ILogger<TelemetriaMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Intercepta todas as requisições HTTP para coletar métricas
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
        var ipCliente = ExtrairIpCliente(context);
        var userAgent = ExtrairUserAgent(context);

        try
        {
            // Processar requisição normalmente
            await _next(context);
        }
        finally
        {
            // Parar cronômetro imediatamente após processamento
            stopwatch.Stop();

            // 🔥 FIRE-AND-FORGET: Registrar métrica em background
            // Resolver o serviço scoped do ServiceProvider em tempo de execução
            _ = RegistrarMetricaAsync(
                context,
                nomeApi,
                endpoint,
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode >= 200 && context.Response.StatusCode < 300,
                context.Response.StatusCode,
                ipCliente,
                userAgent,
                context.RequestAborted
            );
        }
    }

    /// <summary>
    /// Registra métrica de forma assíncrona sem bloquear o thread principal
    /// Resolve o serviço scoped em tempo de execução para evitar problemas de DI
    /// </summary>
    private async Task RegistrarMetricaAsync(
        HttpContext context,
        string nomeApi,
        string endpoint,
        long tempoResposta,
        bool sucesso,
        int statusCode,
        string? ipCliente,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        try
        {
            // Resolver o serviço scoped do ServiceProvider em tempo de execução
            // Isso é necessário porque middlewares são singletons
            var telemetriaService = context.RequestServices.GetRequiredService<ITelemetriaService>();
            
            await telemetriaService.RegistrarMetricaAsync(
                nomeApi,
                endpoint,
                tempoResposta,
                sucesso,
                statusCode,
                ipCliente,
                userAgent,
                cancellationToken);
        }
        catch (Exception ex)
        {
            // Log apenas em nível debug para não poluir logs em produção
            _logger.LogDebug(ex, "Falha ao registrar métrica de telemetria para {Endpoint}", endpoint);
        }
    }

    /// <summary>
    /// Extrai o nome da API baseado no controlador
    /// </summary>
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

    /// <summary>
    /// Extrai IP real do cliente considerando proxies e load balancers
    /// </summary>
    private static string? ExtrairIpCliente(HttpContext context)
    {
        // Headers comuns para IP real em ambientes com proxy/load balancer
        var headersIp = new[]
        {
            "X-Forwarded-For",
            "X-Real-IP", 
            "X-Client-IP",
            "CF-Connecting-IP" // Cloudflare
        };

        foreach (var header in headersIp)
        {
            if (context.Request.Headers.TryGetValue(header, out StringValues values))
            {
                var ip = values.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
                if (!string.IsNullOrWhiteSpace(ip) && ip != "unknown")
                {
                    return ip;
                }
            }
        }

        // Fallback para IP da conexão direta
        return context.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// Extrai User-Agent do cliente
    /// </summary>
    private static string? ExtrairUserAgent(HttpContext context)
    {
        return context.Request.Headers.TryGetValue("User-Agent", out StringValues userAgent) 
            ? userAgent.FirstOrDefault() 
            : null;
    }

    /// <summary>
    /// Verifica se o endpoint deve ser ignorado na telemetria
    /// </summary>
    private static bool DeveIgnorarEndpoint(PathString path)
    {
        var pathValue = path.Value;
        if (string.IsNullOrWhiteSpace(pathValue))
            return true;

        return EndpointsIgnorados.Any(ignored => 
            pathValue.StartsWith(ignored, StringComparison.OrdinalIgnoreCase));
    }
}
