using FluentValidation;
using Hackathon.Application.Commands;

namespace Hackathon.Application.Validators;

/// <summary>
/// Validator para o comando de realizar simulação
/// </summary>
public class RealizarSimulacaoCommandValidator : AbstractValidator<RealizarSimulacaoCommand>
{
    public RealizarSimulacaoCommandValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero")
            .LessThanOrEqualTo(999_999_999.99m)
            .WithMessage("Valor não pode exceder R$ 999.999.999,99");

        RuleFor(x => x.Prazo)
            .GreaterThan(0)
            .WithMessage("Prazo deve ser maior que zero")
            .LessThanOrEqualTo(600)
            .WithMessage("Prazo não pode exceder 600 meses (50 anos)");
    }
}
