using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Hackathon.API.Constants;

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
    [Range((double)ApiConstraints.ValorMinimoEmprestimo, (double)ApiConstraints.ValorMaximoEmprestimo, ErrorMessage = "Valor inválido")]
    decimal ValorDesejado,
    
    [property: JsonPropertyName("prazo")]
    [property: DefaultValue(5)]
    [Range(ApiConstraints.PrazoMinimoMeses, ApiConstraints.PrazoMaximoMeses, ErrorMessage = "Prazo inválido")]
    int Prazo
);
