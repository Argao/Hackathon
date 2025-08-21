using Hackathon.Application.Results;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Commands;

/// <summary>
/// Comando para realizar uma simulação de crédito
/// </summary>
public sealed record RealizarSimulacaoCommand(
    decimal Valor,
    int Prazo
)
{
    /// <summary>
    /// Valida e converte os dados do comando para Value Objects
    /// </summary>
    public Result<(ValorEmprestimo Valor, PrazoMeses Prazo)> ToValueObjects()
    {
        var valorResult = ValorEmprestimo.Create(Valor);
        if (!valorResult.IsSuccess)
            return Result<(ValorEmprestimo, PrazoMeses)>.Failure(valorResult.Error);

        var prazoResult = PrazoMeses.Create(Prazo);
        if (!prazoResult.IsSuccess)
            return Result<(ValorEmprestimo, PrazoMeses)>.Failure(prazoResult.Error);

        return Result<(ValorEmprestimo, PrazoMeses)>.Success((valorResult.Value, prazoResult.Value));
    }
};
