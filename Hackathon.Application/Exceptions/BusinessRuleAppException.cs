namespace Hackathon.Application.Exceptions;

/// <summary>
/// Exceção utilizada para sinalizar violações de regra de negócio expostas pela camada de aplicação.
/// </summary>
public class BusinessRuleAppException : ApplicationExceptionBase
{
    public string RuleCode { get; }

    public BusinessRuleAppException(string message, string ruleCode)
        : base(message)
    {
        RuleCode = ruleCode;
    }
}

