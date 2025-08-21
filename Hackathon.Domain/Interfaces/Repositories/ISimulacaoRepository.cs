using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

public interface ISimulacaoRepository
{
    Task<Simulacao> AdicionarAsync(Simulacao simulacao, CancellationToken ct);
    Task<IEnumerable<VolumeSimuladoAgregado>> ObterVolumeSimuladoPorProdutoAsync(DateOnly dataReferencia, CancellationToken ct);
    Task<(IEnumerable<Simulacao> Data, int TotalRecords)> ListarPaginadoAsync(int pageNumber, int pageSize, CancellationToken ct);
}
