using Hackathon.API.Contracts.Requests;
using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar simulações de crédito
/// </summary>
/// <remarks>
/// Este controller oferece endpoints para:
/// - Realizar simulações de crédito com diferentes sistemas de amortização
/// - Listar histórico de simulações realizadas
/// - Obter volumes simulados por período
/// </remarks>
[ApiController]
[Route("simulacao")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Simulação")]
public class SimulacaoController(ISimulacaoService simulacaoService) : ControllerBase
{
    /// <summary>
    /// Realiza uma simulação de crédito com sistemas SAC e PRICE
    /// </summary>
    /// <remarks>
    /// Este endpoint calcula uma simulação de crédito considerando:
    /// - Valor desejado para o empréstimo
    /// - Prazo em meses para pagamento
    /// - Sistemas de amortização SAC e PRICE
    /// - Taxa de juros do produto selecionado
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// {
    ///   "valorDesejado": 10000.00,
    ///   "prazo": 12
    /// }
    /// ```
    /// 
    /// **Resposta inclui:**
    /// - Detalhes do produto selecionado
    /// - Parcelas calculadas para cada sistema de amortização
    /// - Valores de amortização, juros e prestação por parcela
    /// </remarks>
    /// <param name="request">Dados da simulação contendo valor desejado e prazo</param>
    /// <param name="ct">Token de cancelamento da operação</param>
    /// <returns>Resultado da simulação com amortizações SAC e PRICE</returns>
    /// <response code="200">Simulação realizada com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="422">Erro de validação nos parâmetros</response>
    /// <response code="500">Erro interno do servidor</response>
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
        var result = await simulacaoService.RealizarSimulacaoAsync(command, ct);
        
        var response = result.Adapt<SimulacaoResponse>();
        
        return Ok(response);
    }
    
    /// <summary>
    /// Lista as simulações realizadas de forma paginada
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna o histórico de simulações realizadas com suporte a paginação.
    /// 
    /// **Resposta inclui:**
    /// - Lista de simulações com detalhes básicos
    /// - Informações de paginação (total, página atual, etc.)
    /// - Metadados da consulta
    /// </remarks>
    /// <param name="request">Parâmetros de paginação e filtros</param>
    /// <param name="ct">Token de cancelamento da operação</param>
    /// <returns>Lista paginada de simulações</returns>
    /// <response code="200">Lista de simulações retornada com sucesso</response>
    /// <response code="400">Parâmetros de paginação inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType<ListarSimulacoesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<object>(StatusCodes.Status500InternalServerError)]
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
    /// <remarks>
    /// Este endpoint retorna estatísticas de volume de simulações realizadas por produto
    /// em uma data específica.
    /// 
    /// **Parâmetros:**
    /// - `dataReferencia`: Data no formato YYYY-MM-DD (ex: 2024-01-15)
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// GET /simulacao/volume-por-dia?dataReferencia=2024-01-15
    /// ```
    /// 
    /// **Resposta inclui:**
    /// - Volume total de simulações por produto
    /// - Data de referência consultada
    /// - Detalhes de cada produto com seus volumes
    /// </remarks>
    /// <param name="dataReferencia">Data de referência para consulta (formato: YYYY-MM-DD)</param>
    /// <param name="ct">Token de cancelamento da operação</param>
    /// <returns>Volume simulado por produto na data especificada</returns>
    /// <response code="200">Volume simulado retornado com sucesso</response>
    /// <response code="400">Data de referência inválida</response>
    /// <response code="404">Nenhum dado encontrado para a data especificada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("volume-por-dia")]
    [ProducesResponseType<VolumeSimuladoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<object>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<object>(StatusCodes.Status500InternalServerError)]
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