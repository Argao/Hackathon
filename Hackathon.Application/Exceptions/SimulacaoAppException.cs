namespace Hackathon.Application.Exceptions;

/// <summary>
/// Exceção de alto nível para falhas na simulação expostas aos consumidores da aplicação.
/// </summary>
public class SimulacaoAppException : ApplicationExceptionBase
{
    public SimulacaoAppException(string message) : base(message)
    {
    }

    public SimulacaoAppException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

