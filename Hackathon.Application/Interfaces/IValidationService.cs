using FluentValidation.Results;

namespace Hackathon.Application.Interfaces;

public interface IValidationService
{
    Task<ValidationResult> ValidateAsync<T>(T request, CancellationToken cancellationToken);
}