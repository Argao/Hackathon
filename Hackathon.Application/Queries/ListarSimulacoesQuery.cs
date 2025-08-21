using Hackathon.Application.Results;

namespace Hackathon.Application.Queries;

/// <summary>
/// Query para listar simulações de forma paginada
/// </summary>
public sealed record ListarSimulacoesQuery(
    int NumeroPagina,
    int TamanhoPagina
)
{
    /// <summary>
    /// Retorna página válida (mínimo 1)
    /// </summary>
    public int GetValidPageNumber() => Math.Max(1, NumeroPagina);
    
    /// <summary>
    /// Retorna tamanho de página válido (entre 1 e 100)
    /// </summary>
    public int GetValidPageSize() => Math.Max(1, Math.Min(100, TamanhoPagina));
};
