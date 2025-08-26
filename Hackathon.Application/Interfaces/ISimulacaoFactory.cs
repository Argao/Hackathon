using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Interfaces;

/// <summary>
/// Factory responsável apenas por criar simulações
/// SRP: Criação de simulações com regras de negócio
/// </summary>
public interface ISimulacaoFactory
{
    Simulacao CriarSimulacao(
        int codigoProduto,
        string descricaoProduto,
        TaxaJuros taxaJuros,
        ValorMonetario valorDesejado,
        PrazoMeses prazoMeses);
}