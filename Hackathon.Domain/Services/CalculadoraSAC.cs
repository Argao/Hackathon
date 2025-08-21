using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Interfaces.Services;

namespace Hackathon.Domain.Services;

public class CalculadoraSAC : ICalculadoraAmortizacao
{
    public SistemaAmortizacao Tipo => SistemaAmortizacao.SAC;
    
    public ResultadoSimulacao Calcular(decimal valorPrincipal, decimal taxaMensal, int prazo)
    {
        var resultado = new ResultadoSimulacao { Tipo = SistemaAmortizacao.SAC };
        var saldoDevedor = valorPrincipal;
        // amortização constante: divide o saldo inicial pelo número de parcelas
        var amortizacaoConstante = decimal.Round(
            valorPrincipal / prazo,
            2,
            MidpointRounding.AwayFromZero);

        decimal valorTotalParcela = 0;

        for (var parcela = 1; parcela <= prazo; parcela++)
        {
            // juros do mês
            var juros = decimal.Round(saldoDevedor * taxaMensal, 2, MidpointRounding.AwayFromZero);
            // valor da prestação varia: amortização constante + juros
            var valorPrestacao = decimal.Round(amortizacaoConstante + juros, 2, MidpointRounding.AwayFromZero);
            // diminui o saldo devedor apenas pela amortização
            saldoDevedor = decimal.Round(saldoDevedor - amortizacaoConstante, 2, MidpointRounding.AwayFromZero);
            
            valorTotalParcela += valorPrestacao;
            
            resultado.Parcelas.Add(new Parcela
            {
                Numero = parcela,
                ValorPrestacao = valorPrestacao,
                ValorAmortizacao = amortizacaoConstante,
                ValorJuros = juros,
            });
        }
        resultado.ValorTotal = valorTotalParcela;
        return resultado;
    }
}
