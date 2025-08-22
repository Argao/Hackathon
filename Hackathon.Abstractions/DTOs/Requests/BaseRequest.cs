using System.ComponentModel.DataAnnotations;

namespace Hackathon.Abstractions.DTOs.Requests;

/// <summary>
/// Classe base para requests com validações comuns
/// </summary>
public abstract class BaseRequest
{
    /// <summary>
    /// Valida o request usando Data Annotations
    /// </summary>
    /// <returns>True se válido, false caso contrário</returns>
    public virtual bool IsValid()
    {
        var validationContext = new ValidationContext(this);
        var validationResults = new List<ValidationResult>();
        
        return Validator.TryValidateObject(this, validationContext, validationResults, true);
    }
    
    /// <summary>
    /// Obtém os erros de validação
    /// </summary>
    /// <returns>Lista de erros de validação</returns>
    public virtual IEnumerable<string> GetValidationErrors()
    {
        var validationContext = new ValidationContext(this);
        var validationResults = new List<ValidationResult>();
        
        Validator.TryValidateObject(this, validationContext, validationResults, true);
        
        return validationResults.Select(vr => vr.ErrorMessage ?? "Erro de validação");
    }
}
