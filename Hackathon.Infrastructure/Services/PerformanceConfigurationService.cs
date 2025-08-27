using Microsoft.Extensions.Logging;
using System.Runtime;

namespace Hackathon.Infrastructure.Services;

/// <summary>
/// Serviço para configurar otimizações de performance do sistema
/// </summary>
public class PerformanceConfigurationService
{
    private readonly ILogger<PerformanceConfigurationService> _logger;

    public PerformanceConfigurationService(ILogger<PerformanceConfigurationService> logger)
    {
        _logger = logger;
    }

    public void ConfigurePerformanceOptimizations()
    {
        try
        {
    

            // ✅ OTIMIZAÇÃO: Configurar ThreadPool para alta concorrência
            ConfigureThreadPool();

            // ✅ OTIMIZAÇÃO: Configurar Garbage Collector
            ConfigureGarbageCollector();


        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao configurar otimizações de performance: {Message}", ex.Message);
        }
    }

    private void ConfigureThreadPool()
    {
        var processorCount = Environment.ProcessorCount;
        
        // ✅ OTIMIZAÇÃO: Configuração mais conservadora para reduzir overhead
        var minWorkerThreads = Math.Min(50, processorCount * 2); // Reduzido de 100 para 50
        var minCompletionPortThreads = Math.Min(50, processorCount * 2); // Reduzido de 100 para 50
        var maxWorkerThreads = processorCount * 8; // Aumentado de 4x para 8x para dar mais flexibilidade
        var maxCompletionPortThreads = processorCount * 8;

        ThreadPool.SetMinThreads(minWorkerThreads, minCompletionPortThreads);
        ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionPortThreads);

        
    }

    private void ConfigureGarbageCollector()
    {
        // ✅ OTIMIZAÇÃO: Configurar GC para melhor performance em alta concorrência
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        
        // ✅ OTIMIZAÇÃO: Log do modo atual do GC
        
    }
}
