using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Interfaces;

public interface ICalculadoraService
{
    List<ResultadoSimulacao> ExecutarCalculos(
        ValorMonetario valorEmprestimo,
        TaxaJuros taxaJuros,
        PrazoMeses prazoMeses);
}