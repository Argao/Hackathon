using Hackathon.API.Contracts.Requests;
using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Commands;
using Hackathon.Application.Queries;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

/// <summary>
/// Controller para simulações de crédito com sistemas SAC e PRICE
/// </summary>
[ApiController]
[Route("simulacao")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Simulação")]
public class SimulacaoController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Realiza simulação de crédito com sistemas SAC e PRICE
    /// </summary>
    /// <param name="request">Dados da simulação contendo valor desejado e prazo</param>
    /// <param name="ct">Token de cancelamento da operação</param>
    /// <returns>Resultado da simulação com amortizações SAC e PRICE</returns>
    [HttpPost]
    [ProducesResponseType<SimulacaoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<object>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SimulacaoResponse>> RealizarSimulacao(
        SimulacaoRequest request, 
        CancellationToken ct)
    {
        var command = request.Adapt<RealizarSimulacaoCommand>();
        var result = await mediator.Send(command, ct);
        
        var response = result.Adapt<SimulacaoResponse>();
        
        return Ok(response);
    }
    
    /// <summary>
    /// Lista simulações realizadas com paginação
    /// </summary>
    /// <param name="request">Parâmetros de paginação e filtros</param>
    /// <param name="ct">Token de cancelamento da operação</param>
    /// <returns>Lista paginada de simulações</returns>
    [HttpGet]
    [ProducesResponseType<ListarSimulacoesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<object>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListarSimulacoesResponse>> ListarSimulacoes(
        [FromQuery] ListarSimulacoesRequest request, 
        CancellationToken ct)
    {
        var query = request.Adapt<ListarSimulacoesQuery>();
        var result = await mediator.Send(query, ct);
        var response = result.Adapt<ListarSimulacoesResponse>();
        return Ok(response);
    }
    
    /// <summary>
    /// Obtém o volume simulado por produto em uma data específica
    /// </summary>
    /// <param name="request">Data de referência para consulta</param>
    /// <param name="ct">Token de cancelamento da operação</param>
    /// <returns>Volume simulado por produto na data especificada</returns>
    [HttpGet("volume-por-dia")]
    [ProducesResponseType<VolumeSimuladoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<object>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<object>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VolumeSimuladoResponse>> ObterVolumePorDia(
        [FromQuery] VolumeSimuladoRequest request, 
        CancellationToken ct)
    {
        var query = new ObterVolumeSimuladoQuery(request.DataReferencia);
        var result = await mediator.Send(query, ct);
        var response = result.Adapt<VolumeSimuladoResponse>();
        return Ok(response);
    }
}
