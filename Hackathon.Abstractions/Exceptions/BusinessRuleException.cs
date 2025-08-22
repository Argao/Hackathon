namespace Hackathon.Abstractions.Exceptions;

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada
/// </summary>
public class BusinessRuleException : DomainException
{
    /// <summary>
    /// Código da regra de negócio violada
    /// </summary>
    public string RuleCode { get; }

    public BusinessRuleException(string message, string ruleCode = "") 
        : base(message)
    {
        RuleCode = ruleCode;
    }

    public BusinessRuleException(string message, Exception innerException, string ruleCode = "") 
        : base(message, innerException)
    {
        RuleCode = ruleCode;
    }
}
