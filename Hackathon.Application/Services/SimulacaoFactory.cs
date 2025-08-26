using Hackathon.Application.Interfaces;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Services;

/// <summary>
/// Factory com responsabilidade única: criar simulações
/// SRP: Apenas criação de simulações com regras de negócio
/// </summary>
public class SimulacaoFactory : ISimulacaoFactory
{
    public Simulacao CriarSimulacao(
        int codigoProduto,
        string descricaoProduto,
        TaxaJuros taxaJuros,
        ValorMonetario valorDesejado,
        PrazoMeses prazoMeses)
    {
        return Simulacao.Create(
            codigoProduto,
            descricaoProduto,
            taxaJuros,
            valorDesejado,
            prazoMeses,
            DateOnly.FromDateTime(DateTime.Today)
        );
    }
}