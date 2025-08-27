using Hackathon.Application.Interfaces;
using Hackathon.Application.Services;
using Microsoft.Extensions.Logging;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.Services;
using Hackathon.Infrastructure.Context;
using Hackathon.Infrastructure.EventHub;
using Hackathon.Infrastructure.Repositories;
using Hackathon.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hackathon.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ✅ OTIMIZAÇÃO: Configurar performance do sistema
        var performanceService = new PerformanceConfigurationService(
            services.BuildServiceProvider().GetRequiredService<ILogger<PerformanceConfigurationService>>());
        performanceService.ConfigurePerformanceOptimizations();

        // SQL Server connection for produtos (Entity Framework) - OTIMIZADO
        var connectionString = configuration.GetConnectionString("ProdutosDb")
                               ?? throw new InvalidOperationException("Connection string 'ProdutosDb' não encontrada.");

        services.AddDbContext<ProdutoDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(10); // AGRESSIVO: Timeout muito baixo para dados estáticos
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 1, maxRetryDelay: TimeSpan.FromSeconds(1), errorNumbersToAdd: null);
            });
            
            // PERFORMANCE: Máxima otimização para dados read-only
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            
            options.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.SensitiveDataLoggingEnabledWarning));
        });

        // SQLite connection for local data - OTIMIZADO
        var localConnectionString = configuration.GetConnectionString("LocalDb")
                                  ?? throw new InvalidOperationException("Connection string 'LocalDb' não encontrada.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(localConnectionString, sqliteOptions =>
            {
                sqliteOptions.CommandTimeout(120); // ✅ OTIMIZAÇÃO: Aumentado para volume alto
                sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); // ✅ OTIMIZAÇÃO: Split queries para melhor performance
                sqliteOptions.MaxBatchSize(100); // ✅ OTIMIZAÇÃO: Otimizar inserções em lote
            });
            
            // ✅ OTIMIZAÇÃO: Configurações para volume alto
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // ✅ OTIMIZAÇÃO: No tracking para consultas de leitura
            
            // PERFORMANCE: Reduzir logging em produção
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            
            // ✅ OTIMIZAÇÃO: Configurar batch size para inserções
            options.ConfigureWarnings(warnings =>
            {
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning);
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.SensitiveDataLoggingEnabledWarning);
            });
        });

        // Repositories 
        services.AddScoped<IProdutoRepository, EfProdutoRepository>();
        
        services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();
        services.AddScoped<IMetricaRepository, MetricaRepository>();

        // Domain Services (Calculators)
        services.AddScoped<ICalculadoraAmortizacao, CalculadoraSAC>();
        services.AddScoped<ICalculadoraAmortizacao, CalculadoraPRICE>();

        // Application Services
        services.AddScoped<ICachedProdutoService, CachedProdutoService>();
        
        // Telemetria Services
        services.AddScoped<ITelemetriaService, TelemetriaService>();

        // EventHub Service - Singleton para reutilizar connection pool
        services.AddSingleton<IEventHubService, EventHubService>();
        
        
        // Cache simples para produtos
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 50;
        });

        // PERFORMANCE: Warm-up service para resolver Cold Start
        services.AddHostedService<WarmupService>();

        // Database Initialization Service
        services.AddScoped<DatabaseInitializationService>();

        return services;
    }
}