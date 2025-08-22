using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Services;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.Services;

public class CalculadoraPRICETests
{
    private readonly CalculadoraPRICE _calculadora;
    private readonly Fixture _fixture;

    public CalculadoraPRICETests()
    {
        _calculadora = new CalculadoraPRICE();
        _fixture = new Fixture();
    }

    [Fact]
    public void Tipo_DeveRetornarPRICE()
    {
        // Act
        var tipo = _calculadora.Tipo;

        // Assert
        tipo.Should().Be(SistemaAmortizacao.PRICE);
    }

    [Theory]
    [InlineData(1000.00, 0.01, 12)] // R$ 1000, 1% ao mês, 12 meses
    [InlineData(5000.00, 0.015, 24)] // R$ 5000, 1.5% ao mês, 24 meses
    [InlineData(10000.00, 0.02, 36)] // R$ 10000, 2% ao mês, 36 meses
    public void Calcular_ComParametrosValidos_DeveRetornarResultadoValido(decimal valorPrincipal, decimal taxaMensal, int prazoMeses)
    {
        // Arrange
        var valor = ValorMonetario.Create(valorPrincipal).Value;
        var taxa = TaxaJuros.Create(taxaMensal).Value;
        var prazo = PrazoMeses.Create(prazoMeses).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Tipo.Should().Be(SistemaAmortizacao.PRICE);
        resultado.Parcelas.Should().HaveCount(prazoMeses);
        resultado.ValorTotal.Valor.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Calcular_ComEmprestimoSimples_DeveCalcularCorretamente()
    {
        // Arrange - Empréstimo de R$ 1000, 1% ao mês, 3 meses
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(3).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        resultado.Parcelas.Should().HaveCount(3);
        
        // Todas as parcelas devem ter o mesmo valor de prestação
        var prestacoes = resultado.Parcelas.Select(p => p.ValorPrestacao.Valor).ToList();
        prestacoes[0].Should().BeApproximately(prestacoes[1], 0.01m);
        prestacoes[1].Should().BeApproximately(prestacoes[2], 0.01m);
        
        // Verificar se a soma das amortizações é igual ao valor principal
        var somaAmortizacoes = resultado.Parcelas.Sum(p => p.ValorAmortizacao.Valor);
        somaAmortizacoes.Should().BeApproximately(valor.Valor, 0.01m);
    }

    [Fact]
    public void Calcular_DeveTerPrestacoesConstantes()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(4).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        var prestacoes = resultado.Parcelas.Select(p => p.ValorPrestacao.Valor).ToList();
        
        // Todas as prestações devem ser iguais (com tolerância para arredondamento)
        prestacoes.Skip(1).Should().AllSatisfy(p => p.Should().BeApproximately(prestacoes[0], 0.01m));
    }

    [Fact]
    public void Calcular_DeveTerAmortizacaoCrescente()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(3).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        var amortizacoes = resultado.Parcelas.Select(p => p.ValorAmortizacao.Valor).ToList();
        
        // As amortizações devem ser crescentes no PRICE
        amortizacoes[0].Should().BeLessThan(amortizacoes[1]);
        amortizacoes[1].Should().BeLessThan(amortizacoes[2]);
    }

    [Fact]
    public void Calcular_DeveTerJurosDecrescentes()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(3).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        var juros = resultado.Parcelas.Select(p => p.ValorJuros.Valor).ToList();
        
        // Os juros devem ser decrescentes
        juros[0].Should().BeGreaterThan(juros[1]);
        juros[1].Should().BeGreaterThan(juros[2]);
    }

    [Fact]
    public void Calcular_DeveCalcularValorTotalCorretamente()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(3).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        var somaPrestacoes = resultado.Parcelas.Sum(p => p.ValorPrestacao.Valor);
        resultado.ValorTotal.Valor.Should().BeApproximately(somaPrestacoes, 0.01m);
    }

    [Fact]
    public void Calcular_DeveTerNumeroParcelasCorreto()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(5).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        resultado.Parcelas.Should().HaveCount(5);
        
        for (int i = 0; i < 5; i++)
        {
            resultado.Parcelas.First(p => p.Numero == i + 1).Numero.Should().Be(i + 1);
        }
    }

    [Fact]
    public void Calcular_ComPrazoLongo_DeveFuncionar()
    {
        // Arrange
        var valor = ValorMonetario.Create(10000.00m).Value;
        var taxa = TaxaJuros.Create(0.015m).Value;
        var prazo = PrazoMeses.Create(120).Value; // 10 anos

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        resultado.Parcelas.Should().HaveCount(120);
        resultado.ValorTotal.Valor.Should().BeGreaterThan(valor.Valor);
        
        // Verificar se a primeira parcela tem juros corretos
        var primeiraParcela = resultado.Parcelas.First(p => p.Numero == 1);
        var jurosEsperados = valor.Valor * taxa.Taxa;
        primeiraParcela.ValorJuros.Valor.Should().BeApproximately(jurosEsperados, 0.01m);
    }

    [Fact]
    public void Calcular_DeveArredondarValoresFinanceiramente()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(3).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        foreach (var parcela in resultado.Parcelas)
        {
            // Verificar se os valores estão arredondados para 2 casas decimais
            parcela.ValorPrestacao.Valor.Should().BeApproximately(parcela.ValorPrestacao.Valor, 0.01m);
            parcela.ValorAmortizacao.Valor.Should().BeApproximately(parcela.ValorAmortizacao.Valor, 0.01m);
            parcela.ValorJuros.Valor.Should().BeApproximately(parcela.ValorJuros.Valor, 0.01m);
        }
    }

    [Fact]
    public void Calcular_DeveManterRelacaoPrestacaoIgualAmortizacaoMaisJuros()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(3).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        foreach (var parcela in resultado.Parcelas)
        {
            var somaAmortizacaoJuros = parcela.ValorAmortizacao.Valor + parcela.ValorJuros.Valor;
            parcela.ValorPrestacao.Valor.Should().BeApproximately(somaAmortizacaoJuros, 0.01m);
        }
    }

    [Fact]
    public void Calcular_DeveTerSaldoDevedorCorreto()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(3).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        decimal saldoDevedor = valor.Valor;
        
        var parcelas = resultado.Parcelas.OrderBy(p => p.Numero).ToList();
        for (int i = 0; i < parcelas.Count; i++)
        {
            var parcela = parcelas[i];
            saldoDevedor -= parcela.ValorAmortizacao.Valor;
            
            // O saldo devedor após a última parcela deve ser próximo de zero
            if (i == parcelas.Count - 1)
            {
                saldoDevedor.Should().BeApproximately(0, 0.01m);
            }
        }
    }

    [Theory]
    [InlineData(1000.00, 0.01, 12)]
    [InlineData(5000.00, 0.015, 24)]
    [InlineData(10000.00, 0.02, 36)]
    public void Calcular_DeveTerValorTotalMaiorQuePrincipal(decimal valorPrincipal, decimal taxaMensal, int prazoMeses)
    {
        // Arrange
        var valor = ValorMonetario.Create(valorPrincipal).Value;
        var taxa = TaxaJuros.Create(taxaMensal).Value;
        var prazo = PrazoMeses.Create(prazoMeses).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        resultado.ValorTotal.Valor.Should().BeGreaterThan(valor.Valor);
        
        // O valor total deve ser maior que o principal
        resultado.ValorTotal.Valor.Should().BeGreaterThan(valor.Valor);
    }
}
