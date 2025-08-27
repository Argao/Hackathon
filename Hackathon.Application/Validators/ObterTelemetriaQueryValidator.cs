using FluentValidation;
using Hackathon.Application.Queries;

namespace Hackathon.Application.Validators;

/// <summary>
/// Validador para query de telemetria
/// </summary>
public class ObterTelemetriaQueryValidator : AbstractValidator<ObterTelemetriaQuery>
{
    public ObterTelemetriaQueryValidator()
    {
        RuleFor(x => x.DataReferencia)
            .NotEmpty()
            .WithMessage("Data de referência é obrigatória")
            .Must(data => data <= DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Data de referência não pode ser futura");
    }
}
