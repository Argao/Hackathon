using Hackathon.Application.Extensions;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Services;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.Services;
using Hackathon.Infrastructure.Context;
using Hackathon.Infrastructure.EventHub;
using Hackathon.Infrastructure.Interfaces;
using Hackathon.Infrastructure.Repositories;
using Hackathon.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar performance do sistema
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
                            sqliteOptions.CommandTimeout(120); // Aumentado para volume alto
            sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); // Split queries para melhor performance
            sqliteOptions.MaxBatchSize(100); // Otimizar inserções em lote
        });
        
        // Configurações para volume alto
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // No tracking para consultas de leitura
            
            // PERFORMANCE: Reduzir logging em produção
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            
            // Configurar batch size para inserções
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
        services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();

        // Adicionar serviços da camada de aplicação (incluindo validadores)
        services.AddApplicationServices();

        return services;
    }
}