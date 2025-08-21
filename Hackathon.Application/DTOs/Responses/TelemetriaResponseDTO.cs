namespace Hackathon.Application.DTOs.Responses;

/// <summary>
/// DTO para resposta de telemetria com dados agregados por data
/// </summary>
public record TelemetriaResponseDTO(
    DateOnly DataReferencia,
    List<TelemetriaEndpointDTO> ListaEndpoints
);

/// <summary>
/// DTO para métricas de um endpoint específico
/// </summary>
public record TelemetriaEndpointDTO(
    string NomeApi,
    string Endpoint,
    int QtdRequisicoes,
    double TempoMedio,
    long TempoMinimo,
    long TempoMaximo,
    double PercentualSucesso
);

/// <summary>
/// DTO simplificado para telemetria agrupada apenas por API (conforme especificação)
/// </summary>
public record TelemetriaApiDTO(
    string NomeApi,
    int QtdRequisicoes,
    double TempoMedio,
    long TempoMinimo,
    long TempoMaximo,
    double PercentualSucesso
);

/// <summary>
/// DTO para resposta final conforme especificação do desafio
/// </summary>
public record TelemetriaFinalResponseDTO(
    DateOnly DataReferencia,
    List<TelemetriaApiDTO> ListaEndpoints
);
