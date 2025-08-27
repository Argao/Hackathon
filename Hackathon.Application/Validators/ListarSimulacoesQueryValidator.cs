using FluentValidation;
using Hackathon.Application.Queries;

namespace Hackathon.Application.Validators;

public class ListarSimulacoesQueryValidator : AbstractValidator<ListarSimulacoesQuery>
{
    public ListarSimulacoesQueryValidator()
    {
        RuleFor(x => x.NumeroPagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Número da página deve ser maior ou igual a 1");

        RuleFor(x => x.TamanhoPagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Tamanho da página deve ser maior ou igual a 1");
    }
}
