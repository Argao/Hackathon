using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

public interface IProdutoRepository
{
    Task<Produto?> GetProdutoAdequadoAsync(decimal valor, int prazo, CancellationToken ct);
}