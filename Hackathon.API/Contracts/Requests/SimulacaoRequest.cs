using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Requests;

/// <summary>
/// Request para simulação de crédito
/// </summary>
public sealed record SimulacaoRequest(
    [property: JsonPropertyName("valorDesejado")]
    [property: DefaultValue(900.00)]
    decimal ValorDesejado,
    
    [property: JsonPropertyName("prazo")]
    [property: DefaultValue(5)]
    int Prazo
);
