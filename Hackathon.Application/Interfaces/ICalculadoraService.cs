using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Interfaces;

/// <summary>
/// Service responsável apenas pelos cálculos de amortização
/// SRP: Execução de cálculos financeiros
/// </summary>
public interface ICalculadoraService
{
    List<ResultadoSimulacao> ExecutarCalculos(
        ValorMonetario valorEmprestimo,
        TaxaJuros taxaJuros,
        PrazoMeses prazoMeses);
}