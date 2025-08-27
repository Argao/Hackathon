using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

public interface ISimulacaoRepository
{
    Task<Simulacao> AdicionarAsync(Simulacao simulacao, CancellationToken ct);
    Task<IEnumerable<VolumeSimuladoProdutoDto>> ObterVolumeSimuladoPorProdutoAsync(DateOnly dataReferencia, CancellationToken ct);
    Task<int> ObterTotalSimulacoesAsync(CancellationToken ct);
    
    // OTIMIZAÇÃO: Método com projeção específica - evita carregar parcelas desnecessárias
    Task<IEnumerable<SimulacaoResumoDto>> ListarSimulacoesOtimizadoAsync(int pageNumber, int pageSize, CancellationToken ct);
}

/// <summary>
/// DTO para resumo de simulação - usado para otimização de consultas
/// </summary>
public record SimulacaoResumoDto(
    Guid Id,
    decimal ValorDesejado,
    int Prazo,
    decimal ValorTotalParcelas
);

/// <summary>
/// DTO para volume simulado por produto - usado para consultas agregadas
/// </summary>
public record VolumeSimuladoProdutoDto(
    int CodigoProduto,
    string DescricaoProduto,
    decimal TaxaMediaJuro,
    decimal ValorMedioPrestacao,
    decimal ValorTotalDesejado,
    decimal ValorTotalCredito
);
