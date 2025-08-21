using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Responses;

/// <summary>
/// Response paginado para listagem de simulações
/// </summary>
public sealed record ListarSimulacoesResponse(
    [property: JsonPropertyName("pagina")]
    int Pagina,
    
    [property: JsonPropertyName("qtdRegistros")]
    int QtdRegistros,
    
    [property: JsonPropertyName("qtdRegistrosPagina")]
    int QtdRegistrosPagina,
    
    [property: JsonPropertyName("registros")]
    IReadOnlyList<SimulacaoResumoResponse> Registros
);

/// <summary>
/// Resumo de uma simulação para listagem
/// </summary>
public sealed record SimulacaoResumoResponse(
    [property: JsonPropertyName("idSimulacao")]
    Guid IdSimulacao,
    
    [property: JsonPropertyName("valorDesejado")]
    decimal ValorDesejado,
    
    [property: JsonPropertyName("prazo")]
    int Prazo,
    
    [property: JsonPropertyName("valorTotalParcelas")]
    decimal ValorTotalParcelas
);
