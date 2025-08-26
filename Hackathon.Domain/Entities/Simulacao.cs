using Hackathon.Domain.Enums;
using Hackathon.Domain.ValueObjects;
using Hackathon.Domain.Exceptions;

namespace Hackathon.Domain.Entities;

public sealed class Simulacao
{
    public Guid IdSimulacao { get; init; } = Guid.NewGuid();
    public int CodigoProduto { get; private set; }
    public string DescricaoProduto { get; private set; } = string.Empty;
    public TaxaJuros TaxaJuros { get; private set; }
    public ValorMonetario ValorDesejado { get; private set; }
    public PrazoMeses PrazoMeses { get; private set; }
    public DateOnly DataReferencia { get; private set; }
    public string EnvelopJson { get; private set; } = string.Empty;
    
    public ICollection<ResultadoSimulacao> Resultados { get; private set; } = new List<ResultadoSimulacao>();

    private Simulacao() { }

    public static Simulacao Create(
        int codigoProduto,
        string descricaoProduto,
        TaxaJuros taxaJuros,
        ValorMonetario valorDesejado,
        PrazoMeses prazoMeses,
        DateOnly dataReferencia)
    {
        if (string.IsNullOrWhiteSpace(descricaoProduto))
            throw new BusinessRuleException("Descrição do produto é obrigatória", "SIM001");

        if (dataReferencia > DateOnly.FromDateTime(DateTime.Today))
            throw new BusinessRuleException("Data de referência não pode ser futura", "SIM002");

        return new Simulacao
        {
            CodigoProduto = codigoProduto,
            DescricaoProduto = descricaoProduto,
            TaxaJuros = taxaJuros,
            ValorDesejado = valorDesejado,
            PrazoMeses = prazoMeses,
            DataReferencia = dataReferencia
        };
    }

    public void DefinirEnvelopJson(string envelopJson)
    {
        if (string.IsNullOrWhiteSpace(envelopJson))
            throw new BusinessRuleException("Envelop JSON é obrigatório", "SIM003");

        EnvelopJson = envelopJson;
    }

    public void AdicionarResultado(ResultadoSimulacao resultado)
    {
        if (resultado == null)
            throw new BusinessRuleException("Resultado não pode ser nulo", "SIM004");

        Resultados.Add(resultado);
    }

    public void AdicionarResultados(IEnumerable<ResultadoSimulacao> resultados)
    {
        if (resultados == null)
            throw new BusinessRuleException("Resultados não podem ser nulos", "SIM005");

        foreach (var resultado in resultados)
        {
            AdicionarResultado(resultado);
        }
    }

    public void SetPrazoMeses(short prazo) => PrazoMeses = PrazoMeses.Create(prazo).Value;
}
