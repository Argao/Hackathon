using Hackathon.Application.Interfaces;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Services;

public class CalculadoraService : ICalculadoraService
{
    private readonly IEnumerable<ICalculadoraAmortizacao> _calculadoras;

    public CalculadoraService(IEnumerable<ICalculadoraAmortizacao> calculadoras)
    {
        _calculadoras = calculadoras;
    }

    public List<ResultadoSimulacao> ExecutarCalculos(
        ValorMonetario valorEmprestimo,
        TaxaJuros taxaJuros,
        PrazoMeses prazoMeses)
    {
        return _calculadoras
            .Select(calculadora => calculadora.Calcular(valorEmprestimo, taxaJuros, prazoMeses))
            .ToList();
    }
}