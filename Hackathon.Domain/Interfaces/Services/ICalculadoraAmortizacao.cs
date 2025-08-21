using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Domain.Interfaces.Services;

public interface ICalculadoraAmortizacao
{
    SistemaAmortizacao Tipo { get; }
    ResultadoSimulacao Calcular(ValorMonetario valorPrincipal, TaxaJuros taxaMensal, PrazoMeses prazo);
}
