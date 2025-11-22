namespace Hackathon.Application.Exceptions;

/// <summary>
/// Exceção lançada quando ocorrem erros de validação no pipeline da aplicação.
/// </summary>
public class ApplicationValidationException : ApplicationExceptionBase
{
    public IReadOnlyCollection<string> Errors { get; }

    public ApplicationValidationException(IEnumerable<string> errors)
        : base("Erro de validação")
    {
        Errors = errors.ToArray();
    }
}
