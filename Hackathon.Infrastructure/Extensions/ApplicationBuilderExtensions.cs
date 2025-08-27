using Hackathon.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseInfrastructureDatabaseInitialization(this IApplicationBuilder app)
    {
        var environment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
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
