namespace Hackathon.Domain.ValueObjects;

/// <summary>
/// Value Object representando um prazo em meses válido
/// </summary>
public readonly record struct PrazoMeses
{
    private const int PRAZO_MINIMO = 1;
    private const int PRAZO_MAXIMO = 600; // 50 anos
    
    public int Meses { get; }

    private PrazoMeses(int meses) => Meses = meses;

    /// <summary>
    /// Cria uma instância de PrazoMeses validando o valor
    /// </summary>
    /// <param name="meses">Prazo em meses a ser validado</param>
    /// <returns>Result contendo o PrazoMeses ou erro de validação</returns>
    public static Result<PrazoMeses> Create(int meses)
    {
        if (meses < PRAZO_MINIMO)
            return Result<PrazoMeses>.Failure($"Prazo deve ser maior ou igual a {PRAZO_MINIMO} mês");

        if (meses > PRAZO_MAXIMO)
            return Result<PrazoMeses>.Failure($"Prazo não pode exceder {PRAZO_MAXIMO} meses (50 anos)");

        return Result<PrazoMeses>.Success(new PrazoMeses(meses));
    }

    /// <summary>
    /// Conversão implícita para int
    /// </summary>
    public static implicit operator int(PrazoMeses prazo) => prazo.Meses;

    /// <summary>
    /// Conversão implícita para short (compatibilidade com entidades existentes)
    /// </summary>
    public static implicit operator short(PrazoMeses prazo) => (short)prazo.Meses;

    public override string ToString() => $"{Meses} {(Meses == 1 ? "mês" : "meses")}";
}
