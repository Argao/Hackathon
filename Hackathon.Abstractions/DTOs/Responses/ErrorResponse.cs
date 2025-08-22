namespace Hackathon.Abstractions.DTOs.Responses;

/// <summary>
/// Response para erros
/// </summary>
public class ErrorResponse : BaseResponse
{
    /// <summary>
    /// Lista de erros de validação
    /// </summary>
    public IEnumerable<string> ValidationErrors { get; set; } = [];
    
    /// <summary>
    /// Detalhes adicionais do erro
    /// </summary>
    public object? Details { get; set; }
    
    /// <summary>
    /// Construtor padrão
    /// </summary>
    public ErrorResponse()
    {
        Success = false;
    }
    
    /// <summary>
    /// Construtor com mensagem de erro
    /// </summary>
    public ErrorResponse(string errorMessage, string? errorCode = null)
    {
        Success = false;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }
    
    /// <summary>
    /// Construtor com erros de validação
    /// </summary>
    public ErrorResponse(IEnumerable<string> validationErrors)
    {
        Success = false;
        ValidationErrors = validationErrors;
        ErrorMessage = "Erros de validação encontrados";
    }
}
