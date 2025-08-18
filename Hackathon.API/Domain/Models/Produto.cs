namespace Hackathon.API.Domain.Models;

public sealed record Produto(
    int Codigo,
    string Descricao,
    decimal TaxaMensal,
    short MinMeses,
    short? MaxMeses,
    decimal MinValor,
    decimal? MaxValor);

// public sealed class Produto
// {
//     public int Codigo { get; init; }
//     public string Descricao { get; init; } = default!;
//     public decimal TaxaMensal { get; init; }
//     public short MinMeses { get; init; }
//     public short? MaxMeses { get; init; }
//     public decimal MinValor { get; init; }
//     public decimal? MaxValor { get; init; }
// }