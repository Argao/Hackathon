using FluentValidation.Results;

namespace Hackathon.Application.Interfaces;

/// <summary>
/// Service responsável apenas por validações
/// SRP: Coordenação de validações
/// OCP: Extensível para novos tipos de validação
/// </summary>
public interface IValidationService
{
    Task<ValidationResult> ValidateAsync<T>(T request, CancellationToken cancellationToken);
}