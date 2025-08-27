using FluentValidation;
using FluentValidation.Results;
using Hackathon.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hackathon.Application.Services;

public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ValidationResult> ValidateAsync<T>(T request, CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        
        if (validator == null)
            return new ValidationResult(); // Válido se não houver validador

        return await validator.ValidateAsync(request, cancellationToken);
    }
}