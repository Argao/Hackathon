using Hackathon.Application.Interfaces;
using Mapster;

namespace Hackathon.Application.Services;

/// <summary>
/// Adapter para Mapster implementando a interface genérica IMapper
/// SRP: Responsabilidade única - adaptar o Mapster para nossa abstração
/// OCP: Se quisermos trocar por AutoMapper, criamos novo adapter sem modificar código existente
/// LSP: Pode ser substituído por qualquer implementação de IMapper
/// DIP: Implementa a abstração IMapper
/// </summary>
public class MapsterAdapter : IMapper
{
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
            return default(TDestination)!;
            
        return source.Adapt<TDestination>();
    }

    public IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> source)
    {
        if (source == null)
            return Enumerable.Empty<TDestination>();
            
        return source.Select(item => item.Adapt<TDestination>());
    }

    public TDestination Map<TSource, TDestination>(TSource source, Action<TSource, TDestination> configure)
    {
        if (source == null)
            return default(TDestination)!;

        var destination = source.Adapt<TDestination>();
        configure?.Invoke(source, destination);
        return destination;
    }
}