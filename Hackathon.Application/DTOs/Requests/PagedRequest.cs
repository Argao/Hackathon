namespace Hackathon.Application.DTOs.Requests;

public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Validação para evitar problemas
    public int GetValidPageNumber() => PageNumber < 1 ? 1 : PageNumber;
    public int GetValidPageSize() => PageSize < 1 ? 10 : Math.Min(PageSize, 100); // Máximo 100 itens
}
