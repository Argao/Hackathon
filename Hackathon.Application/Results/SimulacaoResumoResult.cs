namespace Hackathon.Application.Results;

/// <summary>
/// Resumo de simulação para listagem
/// </summary>
public sealed record SimulacaoResumoResult(
    Guid Id,
    decimal ValorDesejado,
    int Prazo,
    decimal ValorTotalParcelas
);
