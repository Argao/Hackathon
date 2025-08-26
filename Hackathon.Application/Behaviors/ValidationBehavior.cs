using FluentValidation;
using Hackathon.Application.Interfaces;
using MediatR;
using DomainValidationException = Hackathon.Domain.Exceptions.ValidationException;

namespace Hackathon.Application.Behaviors;

/// <summary>
/// Behavior para validação automática de requests
/// Cross-cutting concern aplicado a todos os commands/queries
/// SRP: Apenas validação
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IValidationService _validationService;

    public ValidationBehavior(IValidationService validationService)
    {
        _validationService = validationService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Executar validação automaticamente para qualquer request
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            throw new DomainValidationException(errors);
        }

        // Continuar pipeline se validação passou
        return await next();
    }
}