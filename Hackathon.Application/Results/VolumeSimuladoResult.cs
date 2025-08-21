namespace Hackathon.Application.Results;

/// <summary>
/// Resultado do volume simulado por data
/// </summary>
public sealed record VolumeSimuladoResult(
    DateOnly DataReferencia,
    IReadOnlyList<VolumeSimuladoProdutoResult> Produtos
);

/// <summary>
/// Volume simulado por produto
/// </summary>
public sealed record VolumeSimuladoProdutoResult(
    int CodigoProduto,
    string DescricaoProduto,
    decimal TaxaMediaJuro,
    decimal ValorMedioPrestacao,
    decimal ValorTotalDesejado,
    decimal ValorTotalCredito
);
