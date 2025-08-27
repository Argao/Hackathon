namespace Hackathon.API.Middleware;

/// <summary>
/// Extensões para configurar o pipeline HTTP da aplicação
/// </summary>
public static class PipelineExtensions
{
    /// <summary>
    /// Configura o pipeline HTTP com as configurações de infraestrutura
    /// </summary>
    public static IApplicationBuilder UseInfrastructurePipeline(this IApplicationBuilder app)
    {
        // Configuração de HTTPS redirection baseada no ambiente
        ConfigureHttpsRedirection(app);
        
        // Servir arquivos estáticos (necessário para CSS personalizado do Swagger)
        app.UseStaticFiles();
        
        return app;
    }
    
    /// <summary>
    /// Configura o middleware de inicialização do banco de dados
    /// </summary>
    public static IApplicationBuilder UseDatabaseInitialization(this IApplicationBuilder app)
    {
        return app.UseMiddleware<DatabaseInitializationMiddleware>();
    }
    
    /// <summary>
    /// Configura HTTPS redirection baseado no ambiente
    /// </summary>
    private static void ConfigureHttpsRedirection(IApplicationBuilder app)
    {
        // Desabilitar HTTPS redirection em container (quando DOTNET_RUNNING_IN_CONTAINER=true)
        var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (!isRunningInContainer)
        {
            app.UseHttpsRedirection();
        }
    }
}
