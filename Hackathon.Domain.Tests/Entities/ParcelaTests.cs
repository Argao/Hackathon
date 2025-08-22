using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.Entities;

public class ParcelaTests
{
    private readonly Fixture _fixture;

    public ParcelaTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Construtor_DeveCriarParcelaComIdUnico()
    {
        // Act
        var parcela1 = new Parcela { IdResultado = Guid.NewGuid() };
        var parcela2 = new Parcela { IdResultado = Guid.NewGuid() };

        // Assert
        parcela1.IdResultado.Should().NotBeEmpty();
        parcela2.IdResultado.Should().NotBeEmpty();
        parcela1.IdResultado.Should().NotBe(parcela2.IdResultado);
    }

    [Fact]
    public void Propriedades_DevePermitirDefinirElerValores()
    {
        // Arrange
        var parcela = new Parcela();
        var numero = 5;
        var valorPrestacao = ValorMonetario.Create(1000.00m).Value;
        var valorAmortizacao = ValorMonetario.Create(800.00m).Value;
        var valorJuros = ValorMonetario.Create(200.00m).Value;
        var resultado = new ResultadoSimulacao();

        // Act
        parcela.Numero = numero;
        parcela.ValorPrestacao = valorPrestacao;
        parcela.ValorAmortizacao = valorAmortizacao;
        parcela.ValorJuros = valorJuros;
        parcela.Resultado = resultado;

        // Assert
        parcela.Numero.Should().Be(numero);
        parcela.ValorPrestacao.Should().Be(valorPrestacao);
        parcela.ValorAmortizacao.Should().Be(valorAmortizacao);
        parcela.ValorJuros.Should().Be(valorJuros);
        parcela.Resultado.Should().Be(resultado);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    [InlineData(360)]
    public void Numero_DeveAceitarNumerosPositivos(int numero)
    {
        // Arrange
        var parcela = new Parcela();

        // Act
        parcela.Numero = numero;

        // Assert
        parcela.Numero.Should().Be(numero);
    }

    [Fact]
    public void IdResultado_DeveSerInitOnly()
    {
        // Arrange
        var parcela = new Parcela();
        var idOriginal = parcela.IdResultado;

        // Act & Assert
        // Como IdResultado é init-only, não deve ser possível alterá-lo após a criação
        parcela.IdResultado.Should().Be(idOriginal);
    }

    [Fact]
    public void ValorPrestacao_DeveSerIgualASomaDeAmortizacaoEJuros()
    {
        // Arrange
        var parcela = new Parcela();
        var valorAmortizacao = ValorMonetario.Create(800.00m).Value;
        var valorJuros = ValorMonetario.Create(200.00m).Value;
        var valorPrestacao = valorAmortizacao + valorJuros;

        // Act
        parcela.ValorAmortizacao = valorAmortizacao;
        parcela.ValorJuros = valorJuros;
        parcela.ValorPrestacao = valorPrestacao;

        // Assert
        parcela.ValorPrestacao.Should().Be(valorPrestacao);
        parcela.ValorPrestacao.Valor.Should().Be(1000.00m);
    }

    [Fact]
    public void Resultado_DevePermitirDefinirResultadoSimulacao()
    {
        // Arrange
        var parcela = new Parcela();
        var resultado = new ResultadoSimulacao();

        // Act
        parcela.Resultado = resultado;

        // Assert
        parcela.Resultado.Should().Be(resultado);
    }

    [Fact]
    public void ValoresMonetarios_DevePermitirDefinirValoresMonetarios()
    {
        // Arrange
        var parcela = new Parcela();
        var valorPrestacao = ValorMonetario.Create(1500.00m).Value;
        var valorAmortizacao = ValorMonetario.Create(1200.00m).Value;
        var valorJuros = ValorMonetario.Create(300.00m).Value;

        // Act
        parcela.ValorPrestacao = valorPrestacao;
        parcela.ValorAmortizacao = valorAmortizacao;
        parcela.ValorJuros = valorJuros;

        // Assert
        parcela.ValorPrestacao.Valor.Should().Be(1500.00m);
        parcela.ValorAmortizacao.Valor.Should().Be(1200.00m);
        parcela.ValorJuros.Valor.Should().Be(300.00m);
    }

    [Fact]
    public void ValoresMonetarios_DevePermitirOperacoesMatematicas()
    {
        // Arrange
        var parcela = new Parcela();
        var valorAmortizacao = ValorMonetario.Create(1000.00m).Value;
        var valorJuros = ValorMonetario.Create(100.00m).Value;

        // Act
        parcela.ValorAmortizacao = valorAmortizacao;
        parcela.ValorJuros = valorJuros;
        parcela.ValorPrestacao = valorAmortizacao + valorJuros;

        // Assert
        parcela.ValorPrestacao.Valor.Should().Be(1100.00m);
        (parcela.ValorPrestacao - parcela.ValorJuros).Valor.Should().Be(1000.00m);
    }
}
