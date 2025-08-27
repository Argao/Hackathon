using Hackathon.API.Contracts.Requests;
using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Queries;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

/// <summary>
/// Controller para telemetria e métricas da API
/// </summary>
[ApiController]
[Route("telemetria")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Telemetria")]
public class TelemetriaController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Obtém telemetria agregada por data
    /// </summary>
    /// <param name="request">Data para consulta de métricas</param>
    /// <param name="ct">Token de cancelamento da operação</param>
    /// <returns>Dados de telemetria com volumes e tempos de resposta</returns>
    [HttpGet("por-dia")]
    [ProducesResponseType<TelemetriaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<object>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<object>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TelemetriaResponse>> ObterTelemetriaPorDia(
        [FromQuery] TelemetriaRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var query = new ObterTelemetriaQuery(request.DataReferencia);
            var result = await mediator.Send(query, ct);
            
            var response = result.Adapt<TelemetriaResponse>();
            
            return Ok(response);
        }
        catch (Hackathon.Domain.Exceptions.SimulacaoException ex) when (ex.Message.Contains("Nenhum dado de telemetria encontrado"))
        {
            return NotFound(new 
            { 
                message = "Nenhum dado de telemetria encontrado para a data especificada",
                dataReferencia = request.DataReferencia
            });
        }
    }

    /// <summary>
    /// Endpoint de saúde do serviço de telemetria
    /// </summary>
    /// <returns>Status do serviço de telemetria</returns>
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
