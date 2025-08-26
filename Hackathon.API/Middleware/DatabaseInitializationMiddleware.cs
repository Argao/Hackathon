using Hackathon.Infrastructure.Services;

namespace Hackathon.API.Middleware;

/// <summary>
/// Middleware responsável por inicializar o banco de dados na primeira requisição
/// </summary>
public class DatabaseInitializationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWebHostEnvironment _environment;
    private static bool _isInitialized = false;
    private static readonly object _lock = new object();

    public DatabaseInitializationMiddleware(
        RequestDelegate next, 
        IServiceProvider serviceProvider, 
        IWebHostEnvironment environment)
    {
        _next = next;
        _serviceProvider = serviceProvider;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_isInitialized)
        {
            lock (_lock)
            {
                if (!_isInitialized)
                {
                    _isInitialized = true;
                }
                else
                {
                    return;
                }
            }

            await InitializeDatabaseAsync();
        }

        await _next(context);
    }

    private async Task InitializeDatabaseAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseInitializationMiddleware>>();
        
        try
        {
            logger.LogInformation("🔄 Iniciando inicialização do banco de dados...");
            await dbInitializer.InitializeDatabaseAsync();
            logger.LogInformation("✅ Banco de dados inicializado com sucesso!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Erro crítico ao inicializar banco de dados: {Message}", ex.Message);
            
            // Em desenvolvimento, permitir continuar com erro
            if (_environment.IsDevelopment())
            {
                logger.LogWarning("⚠️ Continuando em modo desenvolvimento apesar do erro...");
            }
            else
            {
                // Em produção, falhar rápido
                logger.LogCritical("💥 Falha crítica na inicialização do banco. Encerrando aplicação.");
                throw;
            }
        }
    }
}
