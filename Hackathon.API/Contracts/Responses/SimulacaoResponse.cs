using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Responses;

/// <summary>
/// Response da simulação de crédito
/// </summary>
public sealed record SimulacaoResponse(
    [property: JsonPropertyName("idSimulacao")]
    Guid IdSimulacao,
    
    [property: JsonPropertyName("codigoProduto")]
    int CodigoProduto,
    
    [property: JsonPropertyName("descricaoProduto")]
    string DescricaoProduto,
    
    [property: JsonPropertyName("taxaJuros")]
    decimal TaxaJuros,
    
    [property: JsonPropertyName("resultadoSimulacao")]
    IReadOnlyList<ResultadoSimulacaoResponse> ResultadoSimulacao
);

/// <summary>
/// Resultado da simulação por tipo de amortização
/// </summary>
public sealed record ResultadoSimulacaoResponse(
    [property: JsonPropertyName("tipo")]
    string Tipo,
    
    [property: JsonPropertyName("parcelas")]
    IReadOnlyList<ParcelaResponse> Parcelas
);

/// <summary>
/// Informações de uma parcela
/// </summary>
public sealed record ParcelaResponse(
    [property: JsonPropertyName("numero")]
    int Numero,
    
    [property: JsonPropertyName("valorAmortizacao")]
    decimal ValorAmortizacao,
    
    [property: JsonPropertyName("valorJuros")]
    decimal ValorJuros,
    
    [property: JsonPropertyName("valorPrestacao")]
    decimal ValorPrestacao
);
