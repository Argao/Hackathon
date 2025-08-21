using FluentValidation;
using Hackathon.Application.Queries;

namespace Hackathon.Application.Validators;

/// <summary>
/// Validator para a query de listagem de simulações
/// </summary>
public class ListarSimulacoesQueryValidator : AbstractValidator<ListarSimulacoesQuery>
{
    public ListarSimulacoesQueryValidator()
    {
        RuleFor(x => x.NumeroPagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Número da página deve ser maior ou igual a 1");

        RuleFor(x => x.TamanhoPagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Tamanho da página deve ser maior ou igual a 1")
            .LessThanOrEqualTo(100)
            .WithMessage("Tamanho da página não pode exceder 100 itens");
    }
}
