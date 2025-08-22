using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Responses;

/// <summary>
/// Response do volume simulado por dia
/// </summary>
/// <remarks>
/// Este contrato representa estatísticas agregadas de simulações realizadas em uma data específica.
/// Fornece insights sobre volumes e valores médios por produto.
/// 
/// **Propriedades:**
/// - `DataReferencia`: Data de referência para as estatísticas
/// - `Simulacoes`: Lista de estatísticas por produto
/// </remarks>
public sealed record VolumeSimuladoResponse(
    [property: JsonPropertyName("dataReferencia")]
    string DataReferencia,
    
    [property: JsonPropertyName("simulacoes")]
    IReadOnlyList<VolumeSimuladoProdutoResponse> Simulacoes
);

/// <summary>
/// Volume simulado por produto
/// </summary>
/// <remarks>
/// Representa estatísticas agregadas de simulações para um produto específico.
/// Inclui valores médios e totais calculados a partir das simulações realizadas.
/// 
/// **Propriedades:**
/// - `CodigoProduto`: Código do produto
/// - `DescricaoProduto`: Descrição do produto
/// - `TaxaMediaJuro`: Taxa média de juros das simulações
/// - `ValorMedioPrestacao`: Valor médio das prestações
/// - `ValorTotalDesejado`: Valor total desejado em todas as simulações
/// - `ValorTotalCredito`: Valor total de crédito concedido
/// </remarks>
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
