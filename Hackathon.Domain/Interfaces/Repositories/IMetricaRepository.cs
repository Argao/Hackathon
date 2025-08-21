using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

/// <summary>
/// Interface para repositório de métricas de telemetria
/// </summary>
public interface IMetricaRepository
{
    /// <summary>
    /// Salva uma métrica de requisição
    /// Método fire-and-forget - não deve lançar exceções
    /// </summary>
    /// <param name="metrica">Métrica a ser salva</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SalvarMetricaAsync(MetricaRequisicao metrica, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtém métricas agregadas por data de referência
    /// </summary>
    /// <param name="dataReferencia">Data para buscar métricas</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de métricas agregadas por endpoint</returns>
    Task<List<MetricaAgregada>> ObterMetricasPorDataAsync(DateOnly dataReferencia, CancellationToken cancellationToken = default);
}

/// <summary>
/// Representa métricas agregadas de um endpoint em uma data específica
/// </summary>
public class MetricaAgregada
{
    public string NomeApi { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public int QtdRequisicoes { get; set; }
    public double TempoMedio { get; set; }
    public long TempoMinimo { get; set; }
    public long TempoMaximo { get; set; }
    public double PercentualSucesso { get; set; }
}
