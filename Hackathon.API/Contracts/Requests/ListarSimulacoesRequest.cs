using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Requests;

/// <summary>
/// Request para listagem paginada de simulações
/// </summary>
/// <remarks>
/// Este contrato define os parâmetros de paginação para listar simulações realizadas.
/// Permite controlar a quantidade de registros retornados e a página desejada.
/// 
/// **Parâmetros:**
/// - `Pagina`: Número da página desejada (deve ser maior que zero)
/// - `QtdRegistrosPagina`: Quantidade de registros por página (deve ser maior que zero)
/// </remarks>
public sealed record ListarSimulacoesRequest(
    [property: JsonPropertyName("pagina")]
    [property: DefaultValue(1)]
    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior que zero")]
    int Pagina = 1,
    
    [property: JsonPropertyName("qtdRegistrosPagina")]
    [property: DefaultValue(10)]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade de registros deve ser maior que zero")]
    int QtdRegistrosPagina = 10
);
