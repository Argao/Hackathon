namespace Hackathon.Domain.Entities;

/// <summary>
/// Entidade read-only que representa um produto financeiro do banco externo.
/// Simples e pragmática - dados já validados na origem.
/// </summary>
public sealed class Produto
{
    public int Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal TaxaMensal { get; set; }
    public short MinMeses { get; set; }
    public short? MaxMeses { get; set; }
    public decimal MinValor { get; set; }
    public decimal? MaxValor { get; set; }

    /// <summary>
    /// Regra de negócio: valida se o produto atende aos critérios de empréstimo
    /// </summary>
    public bool AtendeRequisitos(decimal valor, int prazo)
    {
        return valor >= MinValor &&
               (MaxValor == null || valor <= MaxValor) &&
               prazo >= MinMeses &&
               (MaxMeses == null || prazo <= MaxMeses);
    }
}