using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Interfaces.Repositories;
using MediatR;

namespace Hackathon.Application.Handlers;

/// <summary>
/// Handler para listagem de simulações - OTIMIZADO
/// SRP: Coordena consulta otimizada e mapeamento direto
/// </summary>
public class ListarSimulacoesHandler : IRequestHandler<ListarSimulacoesQuery, PagedResult<SimulacaoResumoResult>>
{
    private readonly ISimulacaoRepository _repository;

    public ListarSimulacoesHandler(ISimulacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SimulacaoResumoResult>> Handle(ListarSimulacoesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.GetValidPageNumber();
        var pageSize = request.GetValidPageSize();

        // OTIMIZAÇÃO: Executar contagem e busca em paralelo
        var totalItemsTask = _repository.ObterTotalSimulacoesAsync(cancellationToken);
        var simulacoesTask = _repository.ListarSimulacoesOtimizadoAsync(pageNumber, pageSize, cancellationToken);

        await Task.WhenAll(totalItemsTask, simulacoesTask);

        var totalItems = await totalItemsTask;
        var simulacoes = await simulacoesTask;

        // OTIMIZAÇÃO: Mapeamento direto do DTO para Result
        var resumos = simulacoes.Select(s => new SimulacaoResumoResult(
            Id: s.Id,
            ValorDesejado: s.ValorDesejado,
            Prazo: s.Prazo,
            ValorTotalParcelas: s.ValorTotalParcelas
        )).ToList();

        return new PagedResult<SimulacaoResumoResult>(
            Items: resumos,
            TotalItems: totalItems,
            CurrentPage: pageNumber,
            PageSize: pageSize
        );
    }
}