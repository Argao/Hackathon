namespace Hackathon.Application.Results;

public sealed record SimulacaoResumoResult(
    Guid Id,
    decimal ValorDesejado,
    int Prazo,
    IReadOnlyList<ValorTotalAmortizacaoResult> ValorTotalParcelas
);
