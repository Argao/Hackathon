using Hackathon.API.DTOs;
using Hackathon.Application.DTOs.Requests;
using Hackathon.Application.DTOs.Responses;
using Hackathon.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

[ApiController]
[Route("simulacao")]
public class SimulacaoController(ISimulacaoService simulacaoService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SimulacaoResponseDTO>> RealizarSimulacao(SimulacaoRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        // Mapear API DTO para Application DTO
        var applicationRequest = new SimulacaoRequestDTO
        {
            Valor = request.Valor,
            Prazo = request.Prazo
        };
        
        var response = await simulacaoService.RealizarSimulacaoAsync(applicationRequest, ct);
        
        if (response == null)
        {
            return UnprocessableEntity(new
            {
                error = "Nenhum produto disponível atende aos parâmetros informados",
                message = $"Não foi possível encontrar um produto adequado para o valor R$ {request.Valor:F2} e prazo de {request.Prazo} meses",
                valorInformado = request.Valor,
                prazoInformado = request.Prazo
            });
        }
        
        return Ok(response);
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ListarSimulacoesDTO>>> ListarSimulacoes(
        [FromQuery] PagedRequest request, 
        CancellationToken ct)
    {
        var response = await simulacaoService.ListarPaginadoAsync(request, ct);
        return Ok(response);
    }
    
    [HttpGet("volume-por-dia")]
    public async Task<ActionResult<VolumeSimuladoResponseDTO>> ObterVolumePorDia(
        [FromQuery] DateOnly dataReferencia, 
        CancellationToken ct)
    {
        var response = await simulacaoService.ObterVolumeSimuladoPorDiaAsync(dataReferencia, ct);
        return Ok(response);
    }
}
