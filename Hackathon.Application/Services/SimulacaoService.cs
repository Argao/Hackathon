using FluentValidation;
using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Domain.Exceptions;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Services;

/// <summary>
/// Serviço de aplicação para operações de simulação
/// </summary>
public class SimulacaoService : ISimulacaoService
{
    private readonly ISimulacaoRepository _simulacaoRepository;
    private readonly ICachedProdutoService _cachedProdutoService;
    private readonly IEnumerable<ICalculadoraAmortizacao> _calculadoras;
    private readonly IValidator<RealizarSimulacaoCommand> _simulacaoValidator;
    private readonly IValidator<ObterVolumeSimuladoQuery> _volumeValidator;
    private readonly IEventHubService _eventHubService;
    private readonly ILogger<SimulacaoService> _logger;

    public SimulacaoService(
        ICachedProdutoService cachedProdutoService,
        ISimulacaoRepository simulacaoRepository,
        IEnumerable<ICalculadoraAmortizacao> calculadoras,
        IValidator<RealizarSimulacaoCommand> simulacaoValidator,
        IValidator<ObterVolumeSimuladoQuery> volumeValidator,
        IEventHubService eventHubService,
        ILogger<SimulacaoService> logger)
    {
        _cachedProdutoService = cachedProdutoService;
        _simulacaoRepository = simulacaoRepository;
        _calculadoras = calculadoras;
        _simulacaoValidator = simulacaoValidator;
        _volumeValidator = volumeValidator;
        _eventHubService = eventHubService;
        _logger = logger;
    }

    /// <summary>
    /// Executa uma simulação de crédito
    /// </summary>
    public async Task<SimulacaoResult> RealizarSimulacaoAsync(RealizarSimulacaoCommand command, CancellationToken ct)
    {
        var validationResult = await _simulacaoValidator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
            throw new Hackathon.Domain.Exceptions.ValidationException(
                validationResult.Errors.Select(e => e.ErrorMessage));

        var valueObjectsResult = command.ToValueObjects();
        if (!valueObjectsResult.IsSuccess)
            throw new Hackathon.Domain.Exceptions.ValidationException(valueObjectsResult.Error);

        var (valorEmprestimo, prazoMeses) = valueObjectsResult.Value;

        var valorMonetario = ValorMonetario.Create(valorEmprestimo.Valor).Value;
        var produto = await _cachedProdutoService.GetProdutoAdequadoAsync(valorMonetario, prazoMeses, ct);
        if (produto is null)
            throw new SimulacaoException(
                $"Nenhum produto disponível para valor {valorEmprestimo} e prazo {prazoMeses}");

        var simulacao = Simulacao.Create(
            produto.Codigo,
            produto.Descricao,
            produto.TaxaMensal,
            valorMonetario,
            prazoMeses,
            DateOnly.FromDateTime(DateTime.Today)
        );

        var resultados = _calculadoras
            .Select(c => c.Calcular(valorMonetario, produto.TaxaMensal, prazoMeses))
            .ToList();

        simulacao.AdicionarResultados(resultados);

        // Mapear resultado direto dos cálculos antes de persistir
        var result = new SimulacaoResult(
            Id: simulacao.IdSimulacao,
            CodigoProduto: produto.Codigo,
            DescricaoProduto: produto.Descricao,
            TaxaJuros: produto.TaxaMensal,
            Resultados: resultados.Select(r => new ResultadoCalculoAmortizacao(
                TipoAmortizacao: r.Tipo.ToString(),
                Parcelas: r.Parcelas?.Select(p => new ParcelaCalculada(
                    Numero: p.Numero,
                    ValorAmortizacao: p.ValorAmortizacao.Valor,
                    ValorJuros: p.ValorJuros.Valor,
                    ValorPrestacao: p.ValorPrestacao.Valor
                )).ToList() ?? new List<ParcelaCalculada>()
            )).ToList()
        );

        try
        {
            var persistirStart = DateTime.UtcNow;

            // Inicia a tarefa de persistência
            var persistirTask = _simulacaoRepository.AdicionarAsync(simulacao, ct);

            // Usa ThreadPool para processar em background sem overhead de Task.Run
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    _eventHubService.EnviarSimulacao(result);
                    _logger.LogInformation("✅ EventHub enviado com sucesso - ID: {SimulacaoId}", result.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ EventHub falhou em background - ID: {SimulacaoId}", result.Id);
                }
            }, null);

            // Aguarda apenas a persistência (crítica)
            await persistirTask;

            var totalDuration = DateTime.UtcNow - persistirStart;
            _logger.LogInformation("✅ Persistência concluída em {Duration}ms - ID: {SimulacaoId}",
                totalDuration.TotalMilliseconds, result.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🚨 FALHA CRÍTICA na persistência - ID: {SimulacaoId}", result.Id);
            throw;
        }

        return result;
    }

    /// <summary>
    /// Obtém volume simulado por data
    /// </summary>
    public async Task<VolumeSimuladoResult> ObterVolumeSimuladoAsync(ObterVolumeSimuladoQuery query,
        CancellationToken ct)
    {
        var validationResult = await _volumeValidator.ValidateAsync(query, ct);
        if (!validationResult.IsValid)
            throw new Hackathon.Domain.Exceptions.ValidationException(
                validationResult.Errors.Select(e => e.ErrorMessage));

        var dadosAgregados = await _simulacaoRepository.ObterVolumeSimuladoPorProdutoAsync(query.DataReferencia, ct);

        var produtos = dadosAgregados.Adapt<List<VolumeSimuladoProdutoResult>>();

        var result = new VolumeSimuladoResult(
            DataReferencia: query.DataReferencia,
            Produtos: produtos
        );

        return result;
    }
}