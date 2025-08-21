using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Responses;

/// <summary>
/// Response do volume simulado por dia
/// </summary>
public sealed record VolumeSimuladoResponse(
    [property: JsonPropertyName("dataReferencia")]
    string DataReferencia,
    
    [property: JsonPropertyName("simulacoes")]
    IReadOnlyList<VolumeSimuladoProdutoResponse> Simulacoes
);

/// <summary>
/// Volume simulado por produto
/// </summary>
public sealed record VolumeSimuladoProdutoResponse(
    [property: JsonPropertyName("codigoProduto")]
    int CodigoProduto,
    
    [property: JsonPropertyName("descricaoProduto")]
    string DescricaoProduto,
    
    [property: JsonPropertyName("taxaMediaJuro")]
    decimal TaxaMediaJuro,
    
    [property: JsonPropertyName("valorMedioPrestacao")]
    decimal ValorMedioPrestacao,
    
    [property: JsonPropertyName("valorTotalDesejado")]
    decimal ValorTotalDesejado,
    
    [property: JsonPropertyName("valorTotalCredito")]
    decimal ValorTotalCredito
);
