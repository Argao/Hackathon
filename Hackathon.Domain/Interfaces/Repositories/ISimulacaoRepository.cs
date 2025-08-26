using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

public interface ISimulacaoRepository
{
    Task<Simulacao> AdicionarAsync(Simulacao simulacao, CancellationToken ct);
    Task<IEnumerable<VolumeSimuladoAgregado>> ObterVolumeSimuladoPorProdutoAsync(DateOnly dataReferencia, CancellationToken ct);
    Task<(IEnumerable<Simulacao> Data, int TotalRecords)> ListarPaginadoAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task<int> ObterTotalSimulacoesAsync(CancellationToken ct);
    Task<IEnumerable<Simulacao>> ListarSimulacoesAsync(int pageNumber, int pageSize, CancellationToken ct);
    
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
