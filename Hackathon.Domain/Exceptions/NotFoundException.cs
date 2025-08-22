namespace Hackathon.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso não é encontrado
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// Nome do recurso não encontrado
    /// </summary>
    public string ResourceName { get; }
    
    /// <summary>
    /// Identificador do recurso
    /// </summary>
    public object? ResourceId { get; }

    public NotFoundException(string resourceName, object? resourceId = null) 
        : base($"Recurso '{resourceName}' não encontrado{(resourceId != null ? $" com ID: {resourceId}" : "")}")
    {
        ResourceName = resourceName;
        ResourceId = resourceId;
    }

    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
        ResourceName = "Recurso";
    }
}
