namespace Hackathon.Application.Results;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalItems,
    int CurrentPage,
    int PageSize
) where T : class
{
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;
};
