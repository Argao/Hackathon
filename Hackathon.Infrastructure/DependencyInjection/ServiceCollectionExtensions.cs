using FluentValidation;
using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Mappings;
using Hackathon.Application.Queries;
using Hackathon.Application.Services;
using Hackathon.Application.Validators;
using Microsoft.Extensions.Logging;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.Services;
using Hackathon.Infrastructure.Context;
using Hackathon.Infrastructure.EventHub;
using Hackathon.Infrastructure.Repositories;
using Hackathon.Infrastructure.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hackathon.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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
                sqliteOptions.CommandTimeout(30);
            });
            
            // PERFORMANCE CRÍTICA: Configurações para otimizar batch inserts
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            
            // PERFORMANCE: Reduzir logging em produção
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            
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
        services.AddScoped<ISimulacaoService, SimulacaoService>();
        services.AddScoped<ICachedProdutoService, CachedProdutoService>();
        
        // Telemetria Services
        services.AddScoped<ITelemetriaService, TelemetriaService>();

        // EventHub Service - Singleton para reutilizar connection pool
        services.AddSingleton<IEventHubService, EventHubService>();
        
        
        // FluentValidation - OTIMIZAÇÃO: Cache de validators
        services.AddValidatorsFromAssemblyContaining<RealizarSimulacaoCommandValidator>();

        
        services.AddSingleton<IValidator<RealizarSimulacaoCommand>, RealizarSimulacaoCommandValidator>();
        services.AddSingleton<IValidator<ListarSimulacoesQuery>, ListarSimulacoesQueryValidator>();
        services.AddSingleton<IValidator<ObterVolumeSimuladoQuery>, ObterVolumeSimuladoQueryValidator>();

        // Cache simples para produtos
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 50;
        });

        // Mapster Configuration
        services.AddMapster();
        MapsterConfiguration.Configure();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<RealizarSimulacaoCommandValidator>();

        // PERFORMANCE: Warm-up service para resolver Cold Start
        services.AddHostedService<WarmupService>();

        return services;
    }
}