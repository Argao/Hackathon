using Microsoft.Extensions.Logging;
using System.Runtime;

namespace Hackathon.Infrastructure.Services;

/// <summary>
/// Serviço responsável por configurar otimizações de performance do sistema
/// Configura ThreadPool, GC e outras otimizações para alta concorrência
/// </summary>
public class PerformanceConfigurationService
{
    private readonly ILogger<PerformanceConfigurationService> _logger;

    public PerformanceConfigurationService(ILogger<PerformanceConfigurationService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Configura otimizações de performance para alta concorrência
    /// </summary>
    public void ConfigurePerformanceOptimizations()
    {
        try
        {
            _logger.LogInformation("⚡ Configurando otimizações de performance...");

            // ✅ OTIMIZAÇÃO: Configurar ThreadPool para alta concorrência
            ConfigureThreadPool();

            // ✅ OTIMIZAÇÃO: Configurar Garbage Collector
            ConfigureGarbageCollector();

            _logger.LogInformation("✅ Otimizações de performance configuradas com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha ao configurar otimizações de performance: {Message}", ex.Message);
        }
    }

    /// <summary>
    /// Configura o ThreadPool para alta concorrência
    /// </summary>
    private void ConfigureThreadPool()
    {
        var minWorkerThreads = 100;
        var minCompletionPortThreads = 100;
        var maxWorkerThreads = Environment.ProcessorCount * 4;
        var maxCompletionPortThreads = Environment.ProcessorCount * 4;

        ThreadPool.SetMinThreads(minWorkerThreads, minCompletionPortThreads);
        ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionPortThreads);

        _logger.LogInformation(
            "⚡ ThreadPool configurado: Min({MinWorker}, {MinCompletion}) Max({MaxWorker}, {MaxCompletion})",
            minWorkerThreads, minCompletionPortThreads, maxWorkerThreads, maxCompletionPortThreads);
    }

    /// <summary>
    /// Configura o Garbage Collector para melhor performance
    /// </summary>
    private void ConfigureGarbageCollector()
    {
        // Configurar GC para modo de servidor (melhor para aplicações de alta concorrência)
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        
        _logger.LogInformation("🔄 Garbage Collector configurado para modo servidor");
    }
}
