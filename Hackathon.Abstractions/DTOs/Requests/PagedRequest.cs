using System.ComponentModel.DataAnnotations;

namespace Hackathon.Abstractions.DTOs.Requests;

/// <summary>
/// Request base para operações paginadas
/// </summary>
public class PagedRequest : BaseRequest
{
    /// <summary>
    /// Número da página (base 1)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Número da página deve ser maior que zero")]
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Tamanho da página
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Tamanho da página deve estar entre 1 e 1000")]
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Obtém o número da página válido
    /// </summary>
    public int GetValidPageNumber() => PageNumber < 1 ? 1 : PageNumber;
    
    /// <summary>
    /// Obtém o tamanho da página válido
    /// </summary>
    public int GetValidPageSize() => PageSize < 1 ? 10 : Math.Min(PageSize, 1000);
    
    /// <summary>
    /// Calcula o offset para paginação
    /// </summary>
    public int GetOffset() => (GetValidPageNumber() - 1) * GetValidPageSize();
}
