using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Interfaces.Repositories;
using MediatR;

namespace Hackathon.Application.Handlers;

/// <summary>
/// Handler para listagem de simulações
/// SRP: Apenas coordena consulta e mapeamento
/// </summary>
public class ListarSimulacoesHandler : IRequestHandler<ListarSimulacoesQuery, PagedResult<SimulacaoResumoResult>>
{
    private readonly ISimulacaoRepository _repository;
    private readonly IMapper _mapper;

    public ListarSimulacoesHandler(ISimulacaoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<SimulacaoResumoResult>> Handle(ListarSimulacoesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.GetValidPageNumber();
        var pageSize = request.GetValidPageSize();

        var totalItems = await _repository.ObterTotalSimulacoesAsync(cancellationToken);
        var simulacoes = await _repository.ListarSimulacoesAsync(pageNumber, pageSize, cancellationToken);

        // Usar mapper genérico com configuração customizada para lógica complexa
        var resumos = _mapper.MapCollection<Domain.Entities.Simulacao, SimulacaoResumoResult>(
            simulacoes.ToList()
        ).ToList();

        return new PagedResult<SimulacaoResumoResult>(
            Items: resumos,
            TotalItems: totalItems,
            CurrentPage: pageNumber,
            PageSize: pageSize
        );
    }
}