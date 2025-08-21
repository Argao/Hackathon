using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

public interface ISimulacaoRepository
{
    Task<Simulacao> AdicionarAsync(Simulacao simulacao, CancellationToken ct);
    Task<IEnumerable<VolumeSimuladoAgregado>> ObterVolumeSimuladoPorProdutoAsync(DateOnly dataReferencia, CancellationToken ct);
    Task<(IEnumerable<Simulacao> Data, int TotalRecords)> ListarPaginadoAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task<int> ObterTotalSimulacoesAsync(CancellationToken ct);
    Task<IEnumerable<Simulacao>> ListarSimulacoesAsync(int pageNumber, int pageSize, CancellationToken ct);
}
