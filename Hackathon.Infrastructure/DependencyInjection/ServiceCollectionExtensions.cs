using System.Data;
using Hackathon.Domain.Interfaces;
using Hackathon.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hackathon.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ProdutosDb")
                               ?? throw new InvalidOperationException("Connection string 'ProdutosDb' não encontrada.");

        services.AddSingleton<Func<IDbConnection>>(_ =>
        {
            return () => new SqlConnection(connectionString);
        });

        services.AddScoped<IProdutoRepository,SqlServerProdutoRepository>();

        return services;
    }
}