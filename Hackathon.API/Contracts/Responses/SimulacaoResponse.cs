using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Responses;

/// <summary>
/// Response da simulação de crédito
/// </summary>
/// <remarks>
/// Este contrato representa o resultado completo de uma simulação de crédito.
/// Inclui informações do produto selecionado e resultados para diferentes sistemas de amortização.
/// 
/// **Propriedades:**
/// - `IdSimulacao`: Identificador único da simulação
/// - `CodigoProduto`: Código do produto selecionado
/// - `DescricaoProduto`: Descrição do produto
/// - `TaxaJuros`: Taxa de juros mensal do produto
/// - `ResultadoSimulacao`: Lista de resultados por tipo de amortização
/// </remarks>
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
/// <remarks>
/// Representa os resultados calculados para um sistema específico de amortização.
/// Cada resultado contém a lista completa de parcelas com seus valores detalhados.
/// 
/// **Propriedades:**
/// - `Tipo`: Tipo de sistema de amortização (SAC ou PRICE)
/// - `Parcelas`: Lista de parcelas calculadas
/// </remarks>
public sealed record ResultadoSimulacaoResponse(
    [property: JsonPropertyName("tipo")]
    string Tipo,
    
    [property: JsonPropertyName("parcelas")]
    IReadOnlyList<ParcelaResponse> Parcelas
);

/// <summary>
/// Informações de uma parcela
/// </summary>
/// <remarks>
/// Representa os detalhes de uma parcela específica do empréstimo,
/// incluindo valores de amortização, juros e prestação total.
/// 
/// **Propriedades:**
/// - `Numero`: Número sequencial da parcela
/// - `ValorAmortizacao`: Valor da amortização do principal
/// - `ValorJuros`: Valor dos juros da parcela
/// - `ValorPrestacao`: Valor total da prestação
/// </remarks>
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
