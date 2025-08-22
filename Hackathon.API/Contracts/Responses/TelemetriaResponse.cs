using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Responses;

/// <summary>
/// Resposta de telemetria conforme especificação do desafio
/// </summary>
/// <remarks>
/// Este contrato representa métricas de performance da API para uma data específica.
/// Fornece insights sobre tempos de resposta e volumes de requisições por endpoint.
/// 
/// **Propriedades:**
/// - `DataReferencia`: Data de referência para as métricas de telemetria
/// - `ListaEndpoints`: Lista de métricas por endpoint/API
/// </remarks>
public record TelemetriaResponse(
    [property: JsonPropertyName("dataReferencia")]
    DateOnly DataReferencia,
    
    [property: JsonPropertyName("listaEndpoints")]
    List<TelemetriaEndpointResponse> ListaEndpoints
);

/// <summary>
/// Dados de telemetria de um endpoint/API específica
/// </summary>
/// <remarks>
/// Representa métricas de performance para um endpoint específico da API.
/// Inclui estatísticas de tempo de resposta e volumes de requisições.
/// 
/// **Propriedades:**
/// - `NomeApi`: Nome identificador da API/endpoint
/// - `QtdRequisicoes`: Quantidade total de requisições processadas
/// - `TempoMedio`: Tempo médio de resposta em milissegundos
/// - `TempoMinimo`: Tempo mínimo de resposta em milissegundos
/// - `TempoMaximo`: Tempo máximo de resposta em milissegundos
/// - `PercentualSucesso`: Percentual de requisições com sucesso (0-100)
/// </remarks>
public record TelemetriaEndpointResponse(
    [property: JsonPropertyName("nomeApi")]
    string NomeApi,
    
    [property: JsonPropertyName("qtdRequisicoes")]
    int QtdRequisicoes,
    
    [property: JsonPropertyName("tempoMedio")]
    double TempoMedio,
    
    [property: JsonPropertyName("tempoMinimo")]
    long TempoMinimo,
    
    [property: JsonPropertyName("tempoMaximo")]
    long TempoMaximo,
    
    [property: JsonPropertyName("percentualSucesso")]
    double PercentualSucesso
);
