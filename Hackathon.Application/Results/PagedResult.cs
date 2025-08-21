namespace Hackathon.Application.Results;

/// <summary>
/// Resultado paginado genérico
/// </summary>
/// <typeparam name="T">Tipo do item da lista</typeparam>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalItems,
    int CurrentPage,
    int PageSize
) where T : class
{
    /// <summary>
    /// Número total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    
    /// <summary>
    /// Indica se há próxima página
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
    
    /// <summary>
    /// Indica se há página anterior
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;
};
