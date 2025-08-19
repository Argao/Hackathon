using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> GetProdutoAdequadoAsync(decimal valor, int prazo, CancellationToken ct);
}