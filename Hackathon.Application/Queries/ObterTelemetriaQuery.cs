using Hackathon.Application.Results;
using MediatR;

namespace Hackathon.Application.Queries;

/// <summary>
/// Query para obter dados de telemetria por data de referência
/// </summary>
public sealed record ObterTelemetriaQuery(
    DateOnly DataReferencia
) : IRequest<TelemetriaResult>
{
    /// <summary>
    /// Valida se a data de referência é válida
    /// </summary>
    public bool IsValid()
    {
        return DataReferencia <= DateOnly.FromDateTime(DateTime.Now);
    }
    
    /// <summary>
    /// Obtém a data de referência validada
    /// </summary>
    public DateOnly GetValidDataReferencia()
    {
        if (!IsValid())
            throw new ArgumentException("Data de referência não pode ser futura");
            
        return DataReferencia;
    }
};
