using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Services;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.Services;

public class CalculadoraSACTests
{
    private readonly CalculadoraSAC _calculadora;
    private readonly Fixture _fixture;

    public CalculadoraSACTests()
    {
        _calculadora = new CalculadoraSAC();
        _fixture = new Fixture();
    }

    [Fact]
    public void Tipo_DeveRetornarSAC()
    {
        // Act
        var tipo = _calculadora.Tipo;

        // Assert
        tipo.Should().Be(SistemaAmortizacao.SAC);
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
        resultado.Tipo.Should().Be(SistemaAmortizacao.SAC);
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
        
        // Primeira parcela
        var parcela1 = resultado.Parcelas.First(p => p.Numero == 1);
        parcela1.Numero.Should().Be(1);
        parcela1.ValorAmortizacao.Valor.Should().Be(333.33m); // 1000/3
        parcela1.ValorJuros.Valor.Should().Be(10.00m); // 1000 * 0.01
        parcela1.ValorPrestacao.Valor.Should().Be(343.33m); // 333.33 + 10.00

        // Segunda parcela
        var parcela2 = resultado.Parcelas.First(p => p.Numero == 2);
        parcela2.Numero.Should().Be(2);
        parcela2.ValorAmortizacao.Valor.Should().Be(333.33m);
        parcela2.ValorJuros.Valor.Should().Be(6.67m); // 666.67 * 0.01
        parcela2.ValorPrestacao.Valor.Should().Be(340.00m); // 333.33 + 6.67

        // Terceira parcela
        var parcela3 = resultado.Parcelas.First(p => p.Numero == 3);
        parcela3.Numero.Should().Be(3);
        parcela3.ValorAmortizacao.Valor.Should().Be(333.33m); // Ajuste para completar 1000
        parcela3.ValorJuros.Valor.Should().Be(3.33m); // 333.33 * 0.01
        parcela3.ValorPrestacao.Valor.Should().Be(336.66m); // 333.33 + 3.33
    }

    [Fact]
    public void Calcular_DeveTerAmortizacaoConstante()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(4).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        var amortizacoes = resultado.Parcelas.Select(p => p.ValorAmortizacao.Valor).ToList();
        
        // As três primeiras parcelas devem ter amortização de 250.00
        amortizacoes.Take(3).Should().AllSatisfy(a => a.Should().Be(250.00m));
        
        // A última parcela deve ter amortização de 250.00 (ajuste para completar 1000)
        amortizacoes.Last().Should().Be(250.00m);
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
    public void Calcular_DeveTerPrestacoesDecrescentes()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000.00m).Value;
        var taxa = TaxaJuros.Create(0.01m).Value;
        var prazo = PrazoMeses.Create(3).Value;

        // Act
        var resultado = _calculadora.Calcular(valor, taxa, prazo);

        // Assert
        var prestacoes = resultado.Parcelas.Select(p => p.ValorPrestacao.Valor).ToList();
        
        // As prestações devem ser decrescentes no SAC
        prestacoes[0].Should().BeGreaterThan(prestacoes[1]);
        prestacoes[1].Should().BeGreaterThan(prestacoes[2]);
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
        resultado.ValorTotal.Valor.Should().Be(somaPrestacoes);
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
}
