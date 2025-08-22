using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Interfaces;
using Hackathon.Domain.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

/// <summary>
/// Controller para endpoints de telemetria e métricas da API
/// </summary>
/// <remarks>
/// Este controller oferece endpoints para:
/// - Obter métricas de performance da API
/// - Monitorar tempos de resposta
/// - Analisar volumes de requisições por endpoint
/// - Verificar saúde do serviço
/// </remarks>
[ApiController]
[Route("telemetria")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Telemetria")]
public class TelemetriaController : ControllerBase
{
    private readonly ITelemetriaService _telemetriaService;
    private readonly ILogger<TelemetriaController> _logger;

    public TelemetriaController(
        ITelemetriaService telemetriaService,
        ILogger<TelemetriaController> logger)
    {
        _telemetriaService = telemetriaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém dados de telemetria agregados por data de referência
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna métricas detalhadas de performance da API para uma data específica.
    /// 
    /// **Métricas disponíveis:**
    /// - Quantidade de requisições por endpoint
    /// - Tempo médio de resposta
    /// - Tempo mínimo e máximo de resposta
    /// - Percentual de sucesso das requisições
    /// 
    /// **Parâmetros:**
    /// - `dataReferencia`: Data no formato YYYY-MM-DD (ex: 2024-01-15)
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// GET /telemetria/por-dia?dataReferencia=2024-01-15
    /// ```
    /// 
    /// **Resposta inclui:**
    /// - Data de referência consultada
    /// - Lista de endpoints com suas métricas
    /// - Estatísticas agregadas de performance
    /// 
    /// **Observações:**
    /// - Data de referência não pode ser futura
    /// - Retorna 404 se não houver dados para a data especificada
    /// </remarks>
    /// <param name="dataReferencia">Data para consulta de métricas (formato: yyyy-MM-dd)</param>
    /// <param name="ct">Token de cancelamento da operação</param>
    /// <returns>Dados de telemetria com volumes e tempos de resposta</returns>
    /// <response code="200">Dados de telemetria retornados com sucesso</response>
    /// <response code="400">Parâmetros inválidos ou erro na consulta</response>
    /// <response code="404">Nenhum dado encontrado para a data especificada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("por-dia")]
    [ProducesResponseType<TelemetriaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<object>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<object>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TelemetriaResponse>> ObterTelemetriaPorDia(
        [FromQuery] DateOnly dataReferencia,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Consultando telemetria para data: {DataReferencia}", dataReferencia);

        // Validação básica de data
        if (dataReferencia > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new Hackathon.Domain.Exceptions.ValidationException("Data de referência não pode ser futura");
        }

        // Consultar telemetria no serviço
        var result = await _telemetriaService.ObterTelemetriaPorDataAsync(dataReferencia, ct);

        // Se não houver dados, retornar 404 com estrutura vazia
        if (!result.ListaEndpoints.Any())
        {
            _logger.LogInformation("Nenhum dado de telemetria encontrado para data: {DataReferencia}", dataReferencia);
            return NotFound(new 
            { 
                message = "Nenhum dado de telemetria encontrado para a data especificada",
                dataReferencia = dataReferencia
            });
        }

        // Mapear para response da API
        var response = new TelemetriaResponse(
            DataReferencia: result.DataReferencia,
            ListaEndpoints: result.ListaEndpoints.Select(api => new TelemetriaEndpointResponse(
                NomeApi: api.NomeApi,
                QtdRequisicoes: api.QtdRequisicoes,
                TempoMedio: api.TempoMedio,
                TempoMinimo: api.TempoMinimo,
                TempoMaximo: api.TempoMaximo,
                PercentualSucesso: api.PercentualSucesso
            )).ToList()
        );

        _logger.LogInformation("Telemetria consultada com sucesso: {QtdApis} APIs encontradas", 
            response.ListaEndpoints.Count);

        return Ok(response);
    }

    /// <summary>
    /// Endpoint de saúde específico para o serviço de telemetria
    /// </summary>
    /// <remarks>
    /// Este endpoint verifica se o serviço de telemetria está funcionando corretamente.
    /// 
    /// **Informações retornadas:**
    /// - Nome do serviço
    /// - Status de saúde
    /// - Timestamp da verificação
    /// - Versão do serviço
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// GET /telemetria/health
    /// ```
    /// 
    /// **Resposta esperada:**
    /// ```json
    /// {
    ///   "service": "Telemetria",
    ///   "status": "healthy",
    ///   "timestamp": "2024-01-15T10:30:00Z",
    ///   "version": "1.0.0"
    /// }
    /// ```
    /// </remarks>
    /// <returns>Status do serviço de telemetria</returns>
    /// <response code="200">Serviço de telemetria funcionando normalmente</response>
    /// <response code="500">Serviço de telemetria com problemas</response>
    [HttpGet("health")]
    [ProducesResponseType<object>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status500InternalServerError)]
    public ActionResult<object> HealthCheck()
    {
        return Ok(new 
        { 
            service = "Telemetria",
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }
}
