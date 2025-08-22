using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>?> GetAllAsync(CancellationToken ct = default);
}