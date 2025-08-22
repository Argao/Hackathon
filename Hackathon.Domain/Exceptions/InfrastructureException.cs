namespace Hackathon.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando há erros de infraestrutura
/// </summary>
public class InfrastructureException : DomainException
{
    /// <summary>
    /// Tipo de erro de infraestrutura
    /// </summary>
    public string ErrorType { get; }

    public InfrastructureException(string message, string errorType = "Infrastructure") 
        : base(message)
    {
        ErrorType = errorType;
    }

    public InfrastructureException(string message, Exception innerException, string errorType = "Infrastructure") 
        : base(message, innerException)
    {
        ErrorType = errorType;
    }
}
