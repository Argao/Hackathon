namespace Hackathon.Application.Results;

public sealed record SimulacaoResult(
    Guid Id,
    int CodigoProduto,
    string DescricaoProduto,
    decimal TaxaJuros,
    IReadOnlyList<ResultadoCalculoAmortizacao> Resultados
);

public sealed record ResultadoCalculoAmortizacao(
    string TipoAmortizacao,
    IReadOnlyList<ParcelaCalculada> Parcelas
);

public sealed record ParcelaCalculada(
    int Numero,
    decimal ValorAmortizacao,
    decimal ValorJuros,
    decimal ValorPrestacao
);
