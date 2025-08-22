using System.Data;
using System.Reflection;
using Hackathon.API.Mappings;
using Hackathon.API.Middleware;
using Hackathon.Infrastructure.DependencyInjection;
using Hackathon.Infrastructure.Services;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Simulação de Crédito - Hackathon",
        Description = "API para simulação de crédito com diferentes sistemas de amortização (SAC e PRICE). " +
                     "Permite realizar simulações, listar histórico e obter métricas de telemetria.",
        Version = "v1.0.0",
        Contact = new OpenApiContact
        {
            Name = "João Gabriel Fernandes Moniz de Aragão",
            Email = "joao.aragao@caixa.gov.br",
        }
    });

    // Adicionar comentários XML dos controllers
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Configurar tags para organizar os endpoints
    options.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }

        var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });

    // Adicionar descrições para as tags
    options.DocInclusionPredicate((name, api) => true);
    
    // Configurar esquemas de resposta padrão
    options.MapType<DateTime>(() => new OpenApiSchema { Type = "string", Format = "date-time" });
    options.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
    options.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
});

// Configurar Health Checks
builder.Services.AddHealthChecks();

// Configuração da infraestrutura isolada
builder.Services.AddInfrastructure(builder.Configuration);

// Configure API Mappings
ApiMappingProfile.Configure();

TypeAdapterConfig.GlobalSettings.Compile();

var app = builder.Build();

// Aplicar migrations automaticamente na inicialização
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
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
        if (app.Environment.IsDevelopment())
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Simulação de Crédito v1");
        options.RoutePrefix = "swagger"; // Serve o Swagger em /swagger
        options.DocumentTitle = "API de Simulação de Crédito - Documentação";
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        options.DisplayRequestDuration();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });
}

// Desabilitar HTTPS redirection em container (quando DOTNET_RUNNING_IN_CONTAINER=true)
var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (!isRunningInContainer)
{
    app.UseHttpsRedirection();
}


// Servir arquivos estáticos (necessário para CSS personalizado do Swagger)
app.UseStaticFiles();

// 🔥 Middleware de telemetria ANTES do roteamento para capturar todas as requisições
app.UseMiddleware<TelemetriaMiddleware>();

// 🛡️ Global Exception Handler
app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthorization();

app.MapControllers();

// Configurar endpoint de health check
app.MapHealthChecks("/health");

// Redirecionar raiz para Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

await app.RunAsync();