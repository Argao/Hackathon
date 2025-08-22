using FluentAssertions;
using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class ParcelaDTOTests
{
    [Fact]
    public void Numero_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var parcela = new ParcelaDTO();
        const int expectedNumero = 12;

        // Act
        parcela.Numero = expectedNumero;
        var result = parcela.Numero;

        // Assert
        result.Should().Be(expectedNumero);
    }

    [Fact]
    public void ValorAmortizacao_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var parcela = new ParcelaDTO();
        const decimal expectedValorAmortizacao = 1000.50m;

        // Act
        parcela.ValorAmortizacao = expectedValorAmortizacao;
        var result = parcela.ValorAmortizacao;

        // Assert
        result.Should().Be(expectedValorAmortizacao);
    }

    [Fact]
    public void ValorJuros_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var parcela = new ParcelaDTO();
        const decimal expectedValorJuros = 150.25m;

        // Act
        parcela.ValorJuros = expectedValorJuros;
        var result = parcela.ValorJuros;

        // Assert
        result.Should().Be(expectedValorJuros);
    }

    [Fact]
    public void ValorPrestacao_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var parcela = new ParcelaDTO();
        const decimal expectedValorPrestacao = 1150.75m;

        // Act
        parcela.ValorPrestacao = expectedValorPrestacao;
        var result = parcela.ValorPrestacao;

        // Assert
        result.Should().Be(expectedValorPrestacao);
    }
}
