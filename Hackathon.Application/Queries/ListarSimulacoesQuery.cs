using Hackathon.Application.Results;
using MediatR;

namespace Hackathon.Application.Queries;

/// <summary>
/// Query para listar simulações de forma paginada
/// </summary>
public sealed record ListarSimulacoesQuery(
    int NumeroPagina,
    int TamanhoPagina
) : IRequest<PagedResult<SimulacaoResumoResult>>
{
    /// <summary>
    /// Retorna página válida (mínimo 1)
    /// </summary>
    public int GetValidPageNumber() => Math.Max(1, NumeroPagina);
    
    /// <summary>
    /// Retorna tamanho de página válido (mínimo 1)
    /// </summary>
    public int GetValidPageSize() => Math.Max(1, TamanhoPagina);
};
