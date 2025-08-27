using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

public interface ISimulacaoRepository
{
    Task<Simulacao> AdicionarAsync(Simulacao simulacao, CancellationToken ct);
    Task<IEnumerable<VolumeSimuladoProdutoDto>> ObterVolumeSimuladoPorProdutoAsync(DateOnly dataReferencia, CancellationToken ct);
    Task<int> ObterTotalSimulacoesAsync(CancellationToken ct);
    
    Task<IEnumerable<SimulacaoResumoDto>> ListarSimulacoesOtimizadoAsync(int pageNumber, int pageSize, CancellationToken ct);
}

public record SimulacaoResumoDto(
    Guid Id,
    decimal ValorDesejado,
    int Prazo,
    IReadOnlyList<ValorTotalAmortizacaoDto> ValorTotalParcelas
);

public record ValorTotalAmortizacaoDto(
    string TipoAmortizacao,
    decimal ValorTotal
);

public record VolumeSimuladoProdutoDto(
    int CodigoProduto,
    string DescricaoProduto,
    decimal TaxaMediaJuro,
    decimal ValorMedioPrestacao,
    decimal ValorTotalDesejado,
    decimal ValorTotalCredito
);
