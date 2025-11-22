namespace Hackathon.Application.Constants;

/// <summary>
/// Limites expostos para camadas externas consumirem regras de simulação sem depender do domínio.
/// </summary>
public static class SimulacaoLimits
{
    public const decimal ValorMinimoEmprestimo = 0.01m;
    public const decimal ValorMaximoEmprestimo = 999_999_999.99m;
    public const decimal ValorMinimoMonetario = 0.00m;
    public const decimal ValorMaximoMonetario = 999_999_999_999.99m;
    public const int PrazoMinimoMeses = 1;
    public const int PrazoMaximoApiMeses = 360;
}
