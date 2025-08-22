using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Interfaces;
using Hackathon.Abstractions.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

/// <summary>
/// Controller para endpoints de telemetria e métricas da API
/// </summary>
[ApiController]
[Route("telemetria")]
[Produces("application/json")]
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
    /// <param name="dataReferencia">Data para consulta de métricas (formato: yyyy-MM-dd)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados de telemetria com volumes e tempos de resposta</returns>
    /// <response code="200">Dados de telemetria retornados com sucesso</response>
    /// <response code="400">Parâmetros inválidos ou erro na consulta</response>
    /// <response code="404">Nenhum dado encontrado para a data especificada</response>
    [HttpGet("por-dia")]
    [ProducesResponseType<TelemetriaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<object>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TelemetriaResponse>> ObterTelemetriaPorDia(
        [FromQuery] DateOnly dataReferencia,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Consultando telemetria para data: {DataReferencia}", dataReferencia);

        // Validação básica de data
        if (dataReferencia > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new Hackathon.Abstractions.Exceptions.ValidationException("Data de referência não pode ser futura");
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
    /// <returns>Status do serviço de telemetria</returns>
    [HttpGet("health")]
    [ProducesResponseType<object>(StatusCodes.Status200OK)]
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
