namespace Hackathon.Application.Results;

/// <summary>
/// Resultado de uma simulação de crédito
/// </summary>
public sealed record SimulacaoResult(
    Guid Id,
    int CodigoProduto,
    string DescricaoProduto,
    decimal TaxaJuros,
    IReadOnlyList<ResultadoCalculoAmortizacao> Resultados
);

/// <summary>
/// Resultado do cálculo de amortização (SAC ou PRICE)
/// </summary>
public sealed record ResultadoCalculoAmortizacao(
    string TipoAmortizacao,
    IReadOnlyList<ParcelaCalculada> Parcelas
);

/// <summary>
/// Dados de uma parcela calculada
/// </summary>
public sealed record ParcelaCalculada(
    int Numero,
    decimal ValorAmortizacao,
    decimal ValorJuros,
    decimal ValorPrestacao
);
