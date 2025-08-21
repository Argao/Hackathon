using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Requests;

/// <summary>
/// Request para listagem paginada de simulações
/// </summary>
public sealed record ListarSimulacoesRequest(
    [property: JsonPropertyName("pagina")]
    int Pagina = 1,
    
    [property: JsonPropertyName("qtdRegistrosPagina")]
    int QtdRegistrosPagina = 10
);
