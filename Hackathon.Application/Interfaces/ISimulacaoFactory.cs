using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Interfaces;

public interface ISimulacaoFactory
{
    Simulacao CriarSimulacao(
        int codigoProduto,
        string descricaoProduto,
        TaxaJuros taxaJuros,
        ValorMonetario valorDesejado,
        PrazoMeses prazoMeses);
}