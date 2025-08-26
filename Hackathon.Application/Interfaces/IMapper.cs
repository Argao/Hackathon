namespace Hackathon.Application.Interfaces;

/// <summary>
/// Interface genérica para mapeamento entre objetos
/// SRP: Responsabilidade única - apenas mapeamento
/// OCP: Extensível para diferentes implementações (Mapster, AutoMapper, etc.)
/// ISP: Interface pequena e focada apenas no essencial
/// DIP: Abstração que não depende de detalhes de implementação
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Mapeia um objeto de origem para um tipo de destino
    /// </summary>
    /// <typeparam name="TSource">Tipo de origem</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Objeto de origem</param>
    /// <returns>Objeto mapeado para o tipo de destino</returns>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Mapeia uma coleção de objetos
    /// </summary>
    /// <typeparam name="TSource">Tipo de origem</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Coleção de origem</param>
    /// <returns>Coleção mapeada para o tipo de destino</returns>
    IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source);

    /// <summary>
    /// Mapeia com contexto customizado (para casos complexos)
    /// </summary>
    /// <typeparam name="TSource">Tipo de origem</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Objeto de origem</param>
    /// <param name="configure">Ação para configurar o mapeamento</param>
    /// <returns>Objeto mapeado com configuração personalizada</returns>
    TDestination Map<TSource, TDestination>(TSource source, Action<TSource, TDestination> configure);
}