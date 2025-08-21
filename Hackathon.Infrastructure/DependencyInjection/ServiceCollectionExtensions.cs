using System.Data;
using FluentValidation;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Mappings;
using Hackathon.Application.Services;
using Hackathon.Application.Validators;
using Microsoft.Extensions.Logging;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.Services;
using Hackathon.Infrastructure.Context;
using Hackathon.Infrastructure.Repositories;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hackathon.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // SQL Server connection for produtos
        var connectionString = configuration.GetConnectionString("ProdutosDb")
                               ?? throw new InvalidOperationException("Connection string 'ProdutosDb' não encontrada.");

        services.AddSingleton<Func<IDbConnection>>(_ =>
        {
            return () => new SqlConnection(connectionString);
        });

        // SQLite connection for local data
        var localConnectionString = configuration.GetConnectionString("LocalDb")
                                  ?? throw new InvalidOperationException("Connection string 'LocalDb' não encontrada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(localConnectionString));

        // Repositories 
        services.AddScoped<IProdutoRepository>(provider =>
        {
            var connectionFactory = provider.GetRequiredService<Func<IDbConnection>>();
            var logger = provider.GetRequiredService<ILogger<SqlServerProdutoRepository>>();
            return new SqlServerProdutoRepository(connectionFactory, logger);
        });
        
        services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();

        // Domain Services (Calculators)
        services.AddScoped<ICalculadoraAmortizacao, CalculadoraSAC>();
        services.AddScoped<ICalculadoraAmortizacao, CalculadoraPRICE>();

        // Application Services
        services.AddScoped<ISimulacaoService, SimulacaoService>();
        services.AddScoped<ICachedProdutoService, CachedProdutoService>();

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

        return services;
    }
}