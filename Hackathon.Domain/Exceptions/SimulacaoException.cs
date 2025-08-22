namespace Hackathon.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando há erro na simulação
/// </summary>
public class SimulacaoException : DomainException
{
    public SimulacaoException(string message) : base(message)
    {
    }

    public SimulacaoException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
