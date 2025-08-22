using Hackathon.API.Contracts.Requests;
using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

/// <summary>
/// Controller para operações de simulação de crédito
/// </summary>
[ApiController]
[Route("simulacao")]
[Produces("application/json")]
public class SimulacaoController(ISimulacaoService simulacaoService) : ControllerBase
{
    /// <summary>
    /// Realiza uma simulação de crédito
    /// </summary>
    /// <param name="request">Dados da simulação</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Resultado da simulação com amortizações SAC e PRICE</returns>
    [HttpPost]
    [ProducesResponseType<SimulacaoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SimulacaoResponse>> RealizarSimulacao(
        SimulacaoRequest request, 
        CancellationToken ct)
    {
        var command = request.Adapt<RealizarSimulacaoCommand>();
        var result = await simulacaoService.RealizarSimulacaoAsync(command, ct);
        
        // Mapear resultado para response usando mapeamento manual
        var response = new SimulacaoResponse(
            IdSimulacao: result.Id,
            CodigoProduto: result.CodigoProduto,
            DescricaoProduto: result.DescricaoProduto,
            TaxaJuros: result.TaxaJuros,
            ResultadoSimulacao: result.Resultados.Select(r => new ResultadoSimulacaoResponse(
                Tipo: r.TipoAmortizacao,
                Parcelas: r.Parcelas.Select(p => new ParcelaResponse(
                    Numero: p.Numero,
                    ValorAmortizacao: p.ValorAmortizacao,
                    ValorJuros: p.ValorJuros,
                    ValorPrestacao: p.ValorPrestacao
                )).ToList()
            )).ToList()
        );
        return Ok(response);
    }
    
    /// <summary>
    /// Lista as simulações realizadas de forma paginada
    /// </summary>
    /// <param name="request">Parâmetros de paginação</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Lista paginada de simulações</returns>
    [HttpGet]
    [ProducesResponseType<ListarSimulacoesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListarSimulacoesResponse>> ListarSimulacoes(
        [FromQuery] ListarSimulacoesRequest request, 
        CancellationToken ct)
    {
        // Mapear request para query
        var query = request.Adapt<ListarSimulacoesQuery>();
        
        // Executar listagem
        var result = await simulacaoService.ListarSimulacoesAsync(query, ct);
        
        // Mapear resultado para response
        var response = result.Adapt<ListarSimulacoesResponse>();
        return Ok(response);
    }
    
    /// <summary>
    /// Obtém o volume simulado por produto em uma data específica
    /// </summary>
    /// <param name="dataReferencia">Data de referência</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Volume simulado por produto</returns>
    [HttpGet("volume-por-dia")]
    [ProducesResponseType<VolumeSimuladoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VolumeSimuladoResponse>> ObterVolumePorDia(
        [FromQuery] DateOnly dataReferencia, 
        CancellationToken ct)
    {
        // Criar query
        var query = new ObterVolumeSimuladoQuery(dataReferencia);
        
        // Executar consulta
        var result = await simulacaoService.ObterVolumeSimuladoAsync(query, ct);
        
        // Mapear resultado para response
        var response = result.Adapt<VolumeSimuladoResponse>();
        return Ok(response);
    }
}