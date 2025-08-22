using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Requests;

/// <summary>
/// Request para simulação de crédito
/// </summary>
/// <remarks>
/// Este contrato define os parâmetros necessários para realizar uma simulação de crédito.
/// O sistema automaticamente selecionará o produto mais adequado baseado nos parâmetros fornecidos.
/// 
/// **Parâmetros:**
/// - `ValorDesejado`: Valor desejado para o empréstimo em reais (deve ser maior que zero)
/// - `Prazo`: Prazo do empréstimo em meses (deve estar entre 1 e 360)
/// </remarks>
public sealed record SimulacaoRequest(
    [property: JsonPropertyName("valorDesejado")]
    [property: DefaultValue(900.00)]
    [property: Range(1, double.MaxValue, ErrorMessage = "O valor desejado deve ser maior que zero")]
    decimal ValorDesejado,
    
    [property: JsonPropertyName("prazo")]
    [property: DefaultValue(5)]
    [property: Range(1, 360, ErrorMessage = "O prazo deve estar entre 1 e 360 meses")]
    int Prazo
);
