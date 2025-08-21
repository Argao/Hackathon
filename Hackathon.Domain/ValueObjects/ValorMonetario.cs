namespace Hackathon.Domain.ValueObjects;

/// <summary>
/// Value Object representando um valor monetário válido
/// </summary>
public readonly record struct ValorMonetario
{
    private const decimal VALOR_MINIMO = 0.00m;
    private const decimal VALOR_MAXIMO = 999_999_999_999.99m;
    
    public decimal Valor { get; }

    private ValorMonetario(decimal valor) => Valor = valor;

    /// <summary>
    /// Cria uma instância de ValorMonetario validando o valor
    /// </summary>
    /// <param name="valor">Valor monetário</param>
    /// <returns>Result contendo o ValorMonetario ou erro de validação</returns>
    public static Result<ValorMonetario> Create(decimal valor)
    {
        if (valor < VALOR_MINIMO)
            return Result<ValorMonetario>.Failure($"Valor monetário deve ser maior ou igual a {VALOR_MINIMO:C}");

        if (valor > VALOR_MAXIMO)
            return Result<ValorMonetario>.Failure($"Valor monetário não pode exceder {VALOR_MAXIMO:C}");

        return Result<ValorMonetario>.Success(new ValorMonetario(valor));
    }

    /// <summary>
    /// Cria um valor monetário positivo (para valores que não podem ser zero)
    /// </summary>
    /// <param name="valor">Valor monetário</param>
    /// <returns>Result contendo o ValorMonetario ou erro de validação</returns>
    public static Result<ValorMonetario> CreatePositivo(decimal valor)
    {
        if (valor <= 0)
            return Result<ValorMonetario>.Failure("Valor deve ser maior que zero");

        return Create(valor);
    }

    /// <summary>
    /// Valor zero
    /// </summary>
    public static ValorMonetario Zero => new(0);

    /// <summary>
    /// Conversão implícita para decimal
    /// </summary>
    public static implicit operator decimal(ValorMonetario valor) => valor.Valor;

    /// <summary>
    /// Operações matemáticas básicas
    /// </summary>
    public static ValorMonetario operator +(ValorMonetario a, ValorMonetario b) => new(a.Valor + b.Valor);
    public static ValorMonetario operator -(ValorMonetario a, ValorMonetario b) => new(a.Valor - b.Valor);
    public static ValorMonetario operator *(ValorMonetario a, decimal multiplicador) => new(a.Valor * multiplicador);
    public static ValorMonetario operator *(ValorMonetario a, int multiplicador) => new(a.Valor * multiplicador);
    public static ValorMonetario operator /(ValorMonetario a, decimal divisor) => new(a.Valor / divisor);

    /// <summary>
    /// Comparações
    /// </summary>
    public static bool operator >(ValorMonetario a, ValorMonetario b) => a.Valor > b.Valor;
    public static bool operator <(ValorMonetario a, ValorMonetario b) => a.Valor < b.Valor;
    public static bool operator >=(ValorMonetario a, ValorMonetario b) => a.Valor >= b.Valor;
    public static bool operator <=(ValorMonetario a, ValorMonetario b) => a.Valor <= b.Valor;

    /// <summary>
    /// Arredondamento financeiro
    /// </summary>
    public ValorMonetario ArredondarFinanceiro() => new(decimal.Round(Valor, 2, MidpointRounding.AwayFromZero));

    public override string ToString() => Valor.ToString("C");
}
