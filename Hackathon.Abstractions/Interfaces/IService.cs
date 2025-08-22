namespace Hackathon.Abstractions.Interfaces;

/// <summary>
/// Interface base genérica para serviços
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TId">Tipo do identificador</typeparam>
public interface IService<TEntity, TId> where TEntity : class
{
    /// <summary>
    /// Busca uma entidade por ID
    /// </summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Busca todas as entidades
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cria uma nova entidade
    /// </summary>
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Atualiza uma entidade existente
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove uma entidade
    /// </summary>
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
