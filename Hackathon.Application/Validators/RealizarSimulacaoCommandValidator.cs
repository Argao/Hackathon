using FluentValidation;
using Hackathon.Application.Commands;
using Hackathon.Domain.ValueObjects;

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
            .LessThanOrEqualTo(RegrasNegocio.Valores.VALOR_MAXIMO_EMPRESTIMO)
            .WithMessage($"Valor não pode exceder {RegrasNegocio.Valores.VALOR_MAXIMO_EMPRESTIMO:C}");

        RuleFor(x => x.Prazo)
            .GreaterThan(0)
            .WithMessage("Prazo deve ser maior que zero")
            .LessThanOrEqualTo(RegrasNegocio.Prazos.PRAZO_MAXIMO_MESES)
            .WithMessage($"Prazo não pode exceder {RegrasNegocio.Prazos.PRAZO_MAXIMO_MESES} meses");
    }
}
