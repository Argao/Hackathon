using System.Data;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.Services;
using Hackathon.Infrastructure.Repositories;
using Hackathon.Infrastructure.Context;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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
        services.AddScoped<IProdutoRepository, SqlServerProdutoRepository>();
        services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();

        // Domain Services (Calculators)
        services.AddScoped<ICalculadoraAmortizacao, CalculadoraSAC>();
        services.AddScoped<ICalculadoraAmortizacao, CalculadoraPRICE>();

        // Application Services
        services.AddScoped<ISimulacaoService, SimulacaoService>();

        return services;
    }
}