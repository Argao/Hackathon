using FluentValidation;
using Hackathon.Application.Behaviors;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Services;
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
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TelemetriaBehavior<,>));

        // Services com responsabilidade única
        services.AddScoped<ISimulacaoOrchestrator, SimulacaoOrchestrator>();
        services.AddScoped<ITelemetriaOrchestrator, TelemetriaOrchestrator>();
        services.AddScoped<ISimulacaoFactory, SimulacaoFactory>();
        services.AddScoped<ICalculadoraService, CalculadoraService>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        
        // Mapper genérico (SOLID + Clean Architecture)
        services.AddScoped<IMapper, MapsterAdapter>();

        // FluentValidation - busca automaticamente todos os validadores no assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}