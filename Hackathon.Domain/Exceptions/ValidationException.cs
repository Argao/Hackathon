namespace Hackathon.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando há erro de validação
/// </summary>
public class ValidationException : DomainException
{
    public IEnumerable<string> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new[] { message };
    }

    public ValidationException(IEnumerable<string> errors) : base(string.Join("; ", errors))
    {
        Errors = errors;
    }
}
