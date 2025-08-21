namespace Hackathon.Domain.ValueObjects;

/// <summary>
/// Value Object representando um valor de empréstimo válido
/// </summary>
public readonly record struct ValorEmprestimo
{
    private const decimal VALOR_MINIMO = 0.01m;
    private const decimal VALOR_MAXIMO = 999_999_999.99m;
    
    public decimal Valor { get; }

    private ValorEmprestimo(decimal valor) => Valor = valor;

    /// <summary>
    /// Cria uma instância de ValorEmprestimo validando o valor
    /// </summary>
    /// <param name="valor">Valor a ser validado</param>
    /// <returns>Result contendo o ValorEmprestimo ou erro de validação</returns>
    public static Result<ValorEmprestimo> Create(decimal valor)
    {
        if (valor < VALOR_MINIMO)
            return Result<ValorEmprestimo>.Failure($"Valor deve ser maior ou igual a {VALOR_MINIMO:C}");

        if (valor > VALOR_MAXIMO)
            return Result<ValorEmprestimo>.Failure($"Valor não pode exceder {VALOR_MAXIMO:C}");

        return Result<ValorEmprestimo>.Success(new ValorEmprestimo(valor));
    }

    /// <summary>
    /// Conversão implícita para decimal
    /// </summary>
    public static implicit operator decimal(ValorEmprestimo valor) => valor.Valor;

    public override string ToString() => Valor.ToString("C");
}
