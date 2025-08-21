using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Domain.Services;

public class CalculadoraSAC : ICalculadoraAmortizacao
{
    public SistemaAmortizacao Tipo => SistemaAmortizacao.SAC;
    
    public ResultadoSimulacao Calcular(ValorMonetario valorPrincipal, TaxaJuros taxaMensal, PrazoMeses prazo)
    {
        var resultado = new ResultadoSimulacao { Tipo = SistemaAmortizacao.SAC };
        var saldoDevedor = valorPrincipal.Valor;
        
        // amortização constante: divide o saldo inicial pelo número de parcelas
        var amortizacaoConstante = (valorPrincipal / prazo.Meses).ArredondarFinanceiro();

        var valorTotalParcela = ValorMonetario.Zero;

        for (var numeroParcela = 1; numeroParcela <= prazo.Meses; numeroParcela++)
        {
            // juros do mês
            var juros = (ValorMonetario.Create(saldoDevedor).Value * taxaMensal.Taxa).ArredondarFinanceiro();
            
            // valor da prestação varia: amortização constante + juros
            var valorPrestacao = (amortizacaoConstante + juros).ArredondarFinanceiro();
            
            // diminui o saldo devedor apenas pela amortização
            saldoDevedor = decimal.Round(saldoDevedor - amortizacaoConstante.Valor, 2, MidpointRounding.AwayFromZero);
            
            valorTotalParcela += valorPrestacao;
            
            resultado.Parcelas.Add(new Parcela
            {
                Numero = numeroParcela,
                ValorPrestacao = valorPrestacao,
                ValorAmortizacao = amortizacaoConstante,
                ValorJuros = juros,
            });
        }
        
        resultado.ValorTotal = valorTotalParcela;
        return resultado;
    }
}
