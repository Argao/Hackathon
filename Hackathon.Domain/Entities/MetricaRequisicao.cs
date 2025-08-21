namespace Hackathon.Domain.Entities;

/// <summary>
/// Representa uma métrica de requisição para telemetria
/// </summary>
public class MetricaRequisicao
{
    public int Id { get; set; }
    
    /// <summary>
    /// Nome da API/Controller (ex: "Simulacao")
    /// </summary>
    public string NomeApi { get; set; } = string.Empty;
    
    /// <summary>
    /// Endpoint específico (ex: "POST /simulacao")
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;
    
    /// <summary>
    /// Tempo de resposta em millisegundos
    /// </summary>
    public long TempoRespostaMs { get; set; }
    
    /// <summary>
    /// Se a requisição foi bem-sucedida (status 2xx)
    /// </summary>
    public bool Sucesso { get; set; }
    
    /// <summary>
    /// Status code HTTP da resposta
    /// </summary>
    public int StatusCode { get; set; }
    
    /// <summary>
    /// Data e hora da requisição
    /// </summary>
    public DateTime DataHora { get; set; }
    
    /// <summary>
    /// IP do cliente (opcional, para análises futuras)
    /// </summary>
    public string? IpCliente { get; set; }
    
    /// <summary>
    /// User-Agent do cliente (opcional, para análises futuras)
    /// </summary>
    public string? UserAgent { get; set; }
}
