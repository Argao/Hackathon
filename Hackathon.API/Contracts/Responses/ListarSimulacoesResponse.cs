using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Responses;

/// <summary>
/// Response paginado para listagem de simulações
/// </summary>
/// <remarks>
/// Este contrato representa o resultado de uma consulta paginada de simulações.
/// Inclui metadados de paginação e uma lista resumida das simulações encontradas.
/// 
/// **Propriedades:**
/// - `Pagina`: Número da página atual
/// - `QtdRegistros`: Quantidade total de registros encontrados
/// - `QtdRegistrosPagina`: Quantidade de registros por página
/// - `Registros`: Lista de simulações encontradas na página atual
/// </remarks>
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
/// <remarks>
/// Representa um resumo de uma simulação com informações básicas.
/// Utilizado para listagens onde não é necessário mostrar todos os detalhes.
/// 
/// **Propriedades:**
/// - `IdSimulacao`: Identificador único da simulação
/// - `ValorDesejado`: Valor desejado para o empréstimo
/// - `Prazo`: Prazo do empréstimo em meses
/// - `ValorTotalParcelas`: Valor total das parcelas
/// </remarks>
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
