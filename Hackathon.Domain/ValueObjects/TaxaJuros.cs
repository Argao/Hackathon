namespace Hackathon.Domain.ValueObjects;

/// <summary>
/// Value Object representando uma taxa de juros válida
/// </summary>
public readonly record struct TaxaJuros
{
    private const decimal TAXA_MINIMA = 0.000001m;  // 0.0001% - proteção contra taxas zero ou negativas
    private const decimal TAXA_MAXIMA = 0.50m;      // 50% ao mês - proteção contra taxas absurdas
    
    public decimal Taxa { get; }

    private TaxaJuros(decimal taxa) => Taxa = taxa;

    /// <summary>
    /// Cria uma instância de TaxaJuros validando o valor
    /// </summary>
    /// <param name="taxa">Taxa mensal em decimal (ex: 0.015 = 1.5%)</param>
    /// <returns>Result contendo a TaxaJuros ou erro de validação</returns>
    public static Result<TaxaJuros> Create(decimal taxa)
    {
        if (taxa < TAXA_MINIMA)
            return Result<TaxaJuros>.Failure($"Taxa deve ser maior ou igual a {TAXA_MINIMA:P6}");

        if (taxa > TAXA_MAXIMA)
            return Result<TaxaJuros>.Failure($"Taxa não pode exceder {TAXA_MAXIMA:P2} ao mês");

        return Result<TaxaJuros>.Success(new TaxaJuros(taxa));
    }

    /// <summary>
    /// Cria uma taxa a partir de um percentual (ex: 1.5 vira 0.015)
    /// </summary>
    /// <param name="percentual">Percentual (ex: 1.5 para 1.5%)</param>
    /// <returns>Result contendo a TaxaJuros ou erro</returns>
    public static Result<TaxaJuros> CreateFromPercentual(decimal percentual)
    {
        return Create(percentual / 100m);
    }

    /// <summary>
    /// Conversão para percentual (ex: 0.015 vira 1.5)
    /// </summary>
    public decimal ParaPercentual() => Taxa * 100m;

    /// <summary>
    /// Conversão implícita para decimal
    /// </summary>
    public static implicit operator decimal(TaxaJuros taxa) => taxa.Taxa;

    /// <summary>
    /// Operações matemáticas básicas
    /// </summary>
    public static TaxaJuros operator +(TaxaJuros a, TaxaJuros b) => new(a.Taxa + b.Taxa);
    public static TaxaJuros operator -(TaxaJuros a, TaxaJuros b) => new(a.Taxa - b.Taxa);
    public static TaxaJuros operator *(TaxaJuros a, decimal multiplicador) => new(a.Taxa * multiplicador);

    public override string ToString() => Taxa.ToString("P4");
}
