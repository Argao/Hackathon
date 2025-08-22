using Hackathon.Domain.Entities;

namespace Hackathon.Domain.Interfaces.Repositories;

public interface IMetricaRepository
{
    Task SalvarMetricaAsync(MetricaRequisicao metrica, CancellationToken cancellationToken = default);
    
    Task<List<MetricaAgregada>> ObterMetricasPorDataAsync(DateOnly dataReferencia, CancellationToken cancellationToken = default);
}

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
