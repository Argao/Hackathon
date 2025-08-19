namespace Hackathon.Domain.Entities;

public sealed class Produto
{
    public int Codigo { get; private set; }
    public string Descricao { get; private set; }
    public decimal TaxaMensal { get; private set; }
    public short MinMeses { get; private set; }
    public short? MaxMeses { get; private set; }
    public decimal MinValor { get; private set; }
    public decimal? MaxValor { get; private set; }

    public Produto(int codigo, string descricao, decimal taxaMensal, short minMeses, short? maxMeses, decimal minValor, decimal? maxValor)
    {
        Codigo = codigo;
        Descricao = descricao;
        TaxaMensal = taxaMensal;
        MinMeses = minMeses;
        MaxMeses = maxMeses;
        MinValor = minValor;
        MaxValor = maxValor;
    }
}