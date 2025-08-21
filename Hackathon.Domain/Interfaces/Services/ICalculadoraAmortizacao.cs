using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;

namespace Hackathon.Domain.Interfaces.Services;

public interface ICalculadoraAmortizacao
{
    SistemaAmortizacao Tipo { get; }
    ResultadoSimulacao Calcular(decimal valorPrincipal, decimal taxaMensal, int prazo);
}
