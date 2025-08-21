using FluentValidation;
using Hackathon.Application.Queries;

namespace Hackathon.Application.Validators;

/// <summary>
/// Validator para a query de obter volume simulado
/// </summary>
public class ObterVolumeSimuladoQueryValidator : AbstractValidator<ObterVolumeSimuladoQuery>
{
    public ObterVolumeSimuladoQueryValidator()
    {
        RuleFor(x => x.DataReferencia)
            .NotEmpty()
            .WithMessage("Data de referência é obrigatória")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Data de referência não pode ser futura");
    }
}
