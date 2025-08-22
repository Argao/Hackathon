namespace Hackathon.Application.Results;

/// <summary>
/// Resultado de telemetria com dados agregados por data
/// </summary>
public sealed record TelemetriaResult(
    DateOnly DataReferencia,
    IReadOnlyList<TelemetriaApiResult> ListaEndpoints
);

/// <summary>
/// Dados de telemetria de uma API específica
/// </summary>
public sealed record TelemetriaApiResult(
    string NomeApi,
    int QtdRequisicoes,
    double TempoMedio,
    long TempoMinimo,
    long TempoMaximo,
    double PercentualSucesso
);

/// <summary>
/// Dados de telemetria de um endpoint específico
/// </summary>
public sealed record TelemetriaEndpointResult(
    string NomeApi,
    string Endpoint,
    int QtdRequisicoes,
    double TempoMedio,
    long TempoMinimo,
    long TempoMaximo,
    double PercentualSucesso
);
