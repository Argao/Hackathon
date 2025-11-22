using FluentValidation;
using Hackathon.Application.Exceptions;
using Hackathon.Application.Interfaces;
using MediatR;

namespace Hackathon.Application.Behaviors;

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
            throw new ApplicationValidationException(errors);
        }

        // Continuar pipeline se validação passou
        return await next();
    }
}
