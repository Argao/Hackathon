using FluentValidation;
using Hackathon.Application.Behaviors;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Services;
using Hackathon.Application.Mappings;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Hackathon.Application.DependencyInjection;

/// <summary>
/// Configuração de DI seguindo princípios SOLID
/// SRP: Apenas configuração de dependências da camada Application
/// OCP: Extensível através de métodos de extensão
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR com handlers do assembly atual
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Behaviors na ordem correta (importante!)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        // Removido TelemetriaBehavior - apenas telemetria HTTP via middleware

        // Services com responsabilidade única
        services.AddScoped<ISimulacaoOrchestrator, SimulacaoOrchestrator>();
        services.AddScoped<ISimulacaoFactory, SimulacaoFactory>();
        services.AddScoped<ICalculadoraService, CalculadoraService>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<ITelemetriaService, TelemetriaService>();
        
        // Registrar serviço de cache de volume simulado
        services.AddScoped<IVolumeSimuladoCacheService, VolumeSimuladoCacheService>();
        
        // Mapper genérico (SOLID + Clean Architecture)
        services.AddScoped<IMapper, MapsterAdapter>();

        // FluentValidation - busca automaticamente todos os validadores no assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Mapster Configuration
        services.AddMapster();
        MapsterConfiguration.Configure();

        return services;
    }
}