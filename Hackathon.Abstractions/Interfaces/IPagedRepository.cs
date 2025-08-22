namespace Hackathon.Abstractions.Interfaces;

/// <summary>
/// Interface para repositórios com suporte a paginação
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
public interface IPagedRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Busca entidades de forma paginada
    /// </summary>
    /// <param name="pageNumber">Número da página (base 1)</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Tupla com dados e total de registros</returns>
    Task<(IEnumerable<TEntity> Data, int TotalRecords)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtém o total de registros
    /// </summary>
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
}
