namespace Hackathon.Abstractions.DTOs.Responses;

/// <summary>
/// Classe base para responses
/// </summary>
public abstract class BaseResponse
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool Success { get; set; } = true;
    
    /// <summary>
    /// Mensagem de erro (se houver)
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Código de erro (se houver)
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// Timestamp da resposta
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
