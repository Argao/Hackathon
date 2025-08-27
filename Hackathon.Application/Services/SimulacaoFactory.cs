using Hackathon.Application.Interfaces;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Services;

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