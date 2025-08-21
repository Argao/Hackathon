using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

/// <summary>
/// Repository para produtos read-only do SQL Server externo
/// OTIMIZADO: Busca todos os produtos (apenas 4) e usa cache em memória
/// </summary>
public interface IProdutoRepository
{
    /// <summary>
    /// Busca todos os produtos - ÚNICO método necessário (filtro feito em memória)
    /// </summary>
    Task<IEnumerable<Produto>?> GetAllAsync(CancellationToken ct = default);
}