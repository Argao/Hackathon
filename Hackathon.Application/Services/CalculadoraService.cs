using Hackathon.Application.Interfaces;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Services;

/// <summary>
/// Service com responsabilidade única: executar cálculos financeiros
/// SRP: Apenas coordena as calculadoras disponíveis
/// OCP: Extensível para novos tipos de calculadora
/// </summary>
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