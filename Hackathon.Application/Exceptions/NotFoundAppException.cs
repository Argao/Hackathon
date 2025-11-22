namespace Hackathon.Application.Exceptions;

/// <summary>
/// Exceção levantada quando um recurso solicitado não é encontrado na camada de aplicação.
/// </summary>
public class NotFoundAppException : ApplicationExceptionBase
{
    public string? ResourceId { get; }

    public NotFoundAppException(string message, string? resourceId = null) : base(message)
    {
        ResourceId = resourceId;
    }
}

