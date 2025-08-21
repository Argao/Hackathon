using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Interfaces.Services;

namespace Hackathon.Domain.Services;

public class CalculadoraPRICE : ICalculadoraAmortizacao
{
    public SistemaAmortizacao Tipo => SistemaAmortizacao.PRICE;
    
    public ResultadoSimulacao Calcular(decimal valorPrincipal, decimal taxaMensal, int prazo)
    {
        // calcula a prestação fixa usando o saldo original e o prazo total
        var fator = (decimal)Math.Pow((double)(1 + taxaMensal), prazo);
        var valorParcela = decimal.Round(
            valorPrincipal * taxaMensal * fator / (fator - 1),
            2,
            MidpointRounding.AwayFromZero);
        var valorParcelaTotal = valorParcela * prazo;
        
        var resultado = new ResultadoSimulacao { Tipo = SistemaAmortizacao.PRICE };
        var saldoDevedor = valorPrincipal;
       

        for (var parcela = 1; parcela <= prazo; parcela++)
        {
            // juros do mês com arredondamento financeiro
            var juros = decimal.Round(saldoDevedor * taxaMensal, 2, MidpointRounding.AwayFromZero);
            // amortização é a diferença entre a prestação e os juros
            var amortizacao = decimal.Round(valorParcela - juros, 2, MidpointRounding.AwayFromZero);
            // atualiza o saldo devedor mantendo mais casas decimais para evitar erros acumulados
            saldoDevedor = decimal.Round(saldoDevedor - amortizacao, 2, MidpointRounding.AwayFromZero);

            resultado.Parcelas.Add(new Parcela
            {
                Numero = parcela,
                ValorPrestacao = valorParcela,
                ValorAmortizacao = amortizacao,
                ValorJuros = juros,
            });
        }
        resultado.ValorTotal = valorParcelaTotal;
        return resultado;
    }
}
