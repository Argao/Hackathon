namespace Hackathon.Abstractions.Interfaces;

/// <summary>
/// Interface para serviços de cache
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Obtém um valor do cache
    /// </summary>
    /// <typeparam name="T">Tipo do valor</typeparam>
    /// <param name="key">Chave do cache</param>
    /// <returns>Valor do cache ou null se não existir</returns>
    T? Get<T>(string key) where T : class;
    
    /// <summary>
    /// Obtém um valor do cache de forma assíncrona
    /// </summary>
    /// <typeparam name="T">Tipo do valor</typeparam>
    /// <param name="key">Chave do cache</param>
    /// <returns>Valor do cache ou null se não existir</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Define um valor no cache
    /// </summary>
    /// <typeparam name="T">Tipo do valor</typeparam>
    /// <param name="key">Chave do cache</param>
    /// <param name="value">Valor a ser armazenado</param>
    /// <param name="expirationMinutes">Tempo de expiração em minutos</param>
    void Set<T>(string key, T value, int expirationMinutes = 60) where T : class;
    
    /// <summary>
    /// Define um valor no cache de forma assíncrona
    /// </summary>
    /// <typeparam name="T">Tipo do valor</typeparam>
    /// <param name="key">Chave do cache</param>
    /// <param name="value">Valor a ser armazenado</param>
    /// <param name="expirationMinutes">Tempo de expiração em minutos</param>
    Task SetAsync<T>(string key, T value, int expirationMinutes = 60, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Remove um valor do cache
    /// </summary>
    /// <param name="key">Chave do cache</param>
    void Remove(string key);
    
    /// <summary>
    /// Remove um valor do cache de forma assíncrona
    /// </summary>
    /// <param name="key">Chave do cache</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Invalida todo o cache
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Invalida todo o cache de forma assíncrona
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
