using System.Data;
using Hackathon.API.Mappings;
using Hackathon.API.Middleware;
using Hackathon.Infrastructure.DependencyInjection;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Health Checks
builder.Services.AddHealthChecks();

// Configuração da infraestrutura isolada
builder.Services.AddInfrastructure(builder.Configuration);

// Configure API Mappings
ApiMappingProfile.Configure();

TypeAdapterConfig.GlobalSettings.Compile();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔥 Middleware de telemetria ANTES do roteamento para capturar todas as requisições
app.UseMiddleware<TelemetriaMiddleware>();

// 🛡️ Global Exception Handler
app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthorization();

app.MapControllers();

// Configurar endpoint de health check
app.MapHealthChecks("/health");

await app.RunAsync();