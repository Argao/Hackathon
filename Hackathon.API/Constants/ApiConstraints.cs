namespace Hackathon.API.Constants;

/// <summary>
/// Limites expostos pela camada de aplicação; evita depender dos value objects de domínio.
/// </summary>
public static class ApiConstraints
{
    public const decimal ValorMinimoEmprestimo = 0.01m;
    public const decimal ValorMaximoEmprestimo = 999_999_999.99m;
    public const int PrazoMinimoMeses = 1;
    public const int PrazoMaximoMeses = 360;
}

