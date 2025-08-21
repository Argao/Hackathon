using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Responses;

/// <summary>
/// Resposta de telemetria conforme especificação do desafio
/// </summary>
public record TelemetriaResponse(
    [property: JsonPropertyName("dataReferencia")]
    DateOnly DataReferencia,
    
    [property: JsonPropertyName("listaEndpoints")]
    List<TelemetriaEndpointResponse> ListaEndpoints
);

/// <summary>
/// Dados de telemetria de um endpoint/API específica
/// </summary>
public record TelemetriaEndpointResponse(
    [property: JsonPropertyName("nomeApi")]
    string NomeApi,
    
    [property: JsonPropertyName("qtdRequisicoes")]
    int QtdRequisicoes,
    
    [property: JsonPropertyName("tempoMedio")]
    double TempoMedio,
    
    [property: JsonPropertyName("tempoMinimo")]
    long TempoMinimo,
    
    [property: JsonPropertyName("tempoMaximo")]
    long TempoMaximo,
    
    [property: JsonPropertyName("percentualSucesso")]
    double PercentualSucesso
);
