using Hackathon.API.Domain.Models;

namespace Hackathon.API.Persistence.Produtos;

public interface IProdutoReadRepository
{
    Task<Produto?> SelecionarProdutoParaAsync(decimal valor, int prazo, CancellationToken ct);
    Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken ct);
}