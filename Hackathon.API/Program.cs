using System.Reflection;
using Hackathon.API.Mappings;
using Hackathon.API.Middleware;
using Mapster;
using Microsoft.OpenApi.Models;
using Hackathon.Application.Extensions;
using Hackathon.Infrastructure.Extensions;
using Hackathon.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

// Configurar serviços básicos da API
builder.Services.AddControllers();
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

// Configuração da camada Application com SOLID
builder.Services.AddApplicationServices();

// Configure API Mappings
ApiMappingProfile.Configure(TypeAdapterConfig.GlobalSettings);

TypeAdapterConfig.GlobalSettings.Compile();

var app = builder.Build();

// Configurar pipeline de infraestrutura
app.UseInfrastructurePipeline();

// Inicialização do banco baseada no ambiente (encapsulada na infraestrutura)
app.UseInfrastructureDatabaseInitialization();

// Configurar pipeline de requisições HTTP
var enableSwagger = app.Environment.IsDevelopment() || 
                   Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

if (enableSwagger)
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

// Middleware de inicialização do banco de dados
app.UseDatabaseInitialization();

// Middleware de telemetria antes do roteamento
app.UseMiddleware<TelemetriaMiddleware>();

// Global Exception Handler
app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthorization();

app.MapControllers();

// Configurar endpoint de health check
app.MapHealthChecks("/health");

// Redirecionar raiz para Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

await app.RunAsync();
