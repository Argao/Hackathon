namespace Hackathon.Abstractions.DTOs.Responses;

/// <summary>
/// Response base para operações paginadas
/// </summary>
/// <typeparam name="T">Tipo dos dados</typeparam>
public class PagedResponse<T> : BaseResponse
{
    /// <summary>
    /// Dados da página atual
    /// </summary>
    public IEnumerable<T> Data { get; set; } = [];
    
    /// <summary>
    /// Número da página atual
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total de registros
    /// </summary>
    public int TotalRecords { get; set; }
    
    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Indica se existe página anterior
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
    
    /// <summary>
    /// Indica se existe próxima página
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// Construtor padrão
    /// </summary>
    public PagedResponse() { }
    
    /// <summary>
    /// Construtor com parâmetros
    /// </summary>
    public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
    }
}
