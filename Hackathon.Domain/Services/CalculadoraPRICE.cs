using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Domain.Services;

public class CalculadoraPRICE : ICalculadoraAmortizacao
{
    public SistemaAmortizacao Tipo => SistemaAmortizacao.PRICE;
    
    public ResultadoSimulacao Calcular(ValorMonetario valorPrincipal, TaxaJuros taxaMensal, PrazoMeses prazo)
    {
        // calcula a prestação fixa usando o saldo original e o prazo total
        var fator = (decimal)Math.Pow((double)(1 + taxaMensal.Taxa), prazo.Meses);
        var valorParcela = ValorMonetario.Create(
            decimal.Round(
                valorPrincipal.Valor * taxaMensal.Taxa * fator / (fator - 1),
                2,
                MidpointRounding.AwayFromZero)).Value;
        
        var valorParcelaTotal = valorParcela * prazo.Meses;
        
        var resultado = new ResultadoSimulacao { Tipo = SistemaAmortizacao.PRICE };
        var saldoDevedor = valorPrincipal.Valor;

        for (var numeroParcela = 1; numeroParcela <= prazo.Meses; numeroParcela++)
        {
            // juros do mês com arredondamento financeiro
            var juros = (ValorMonetario.Create(saldoDevedor).Value * taxaMensal.Taxa).ArredondarFinanceiro();
            
            // amortização é a diferença entre a prestação e os juros
            var amortizacao = (valorParcela - juros).ArredondarFinanceiro();
            
            // atualiza o saldo devedor mantendo mais casas decimais para evitar erros acumulados
            saldoDevedor = decimal.Round(saldoDevedor - amortizacao.Valor, 2, MidpointRounding.AwayFromZero);

            resultado.Parcelas.Add(new Parcela
            {
                Numero = numeroParcela,
                ValorPrestacao = valorParcela,
                ValorAmortizacao = amortizacao,
                ValorJuros = juros,
            });
        }
        
        resultado.ValorTotal = valorParcelaTotal;
        return resultado;
    }
}
