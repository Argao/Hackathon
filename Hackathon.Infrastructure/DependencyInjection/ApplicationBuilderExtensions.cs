using Hackathon.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.DependencyInjection;

/// <summary>
/// Extensões para configurar o pipeline de aplicação da infraestrutura
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configura a inicialização do banco de dados baseada no ambiente
    /// </summary>
    public static IApplicationBuilder UseInfrastructureDatabaseInitialization(this IApplicationBuilder app)
    {
        var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        var logger = app.ApplicationServices.GetRequiredService<ILogger<IApplicationBuilder>>();

        if (environment.IsProduction())
        {
            try
            {
                logger.LogInformation("Inicializando banco de dados em produção...");
                using var scope = app.ApplicationServices.CreateScope();
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializationService>();
                dbInitializer.InitializeDatabaseAsync().Wait();
                logger.LogInformation("Banco de dados inicializado com sucesso");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Falha crítica na inicialização do banco de dados");
                throw;
            }
        }

        return app;
    }
}
