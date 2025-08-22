using FluentAssertions;
using Hackathon.Application.DTOs.Responses;
using Hackathon.Domain.Enums;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class ValorTotalParcelasDTOTests
{
    [Fact]
    public void Tipo_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var valorTotal = new ValorTotalParcelasDTO();
        const SistemaAmortizacao expectedTipo = SistemaAmortizacao.SAC;

        // Act
        valorTotal.Tipo = expectedTipo;
        var result = valorTotal.Tipo;

        // Assert
        result.Should().Be(expectedTipo);
    }

    [Fact]
    public void ValorTotal_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var valorTotal = new ValorTotalParcelasDTO();
        const decimal expectedValor = 60000.00m;

        // Act
        valorTotal.ValorTotal = expectedValor;
        var result = valorTotal.ValorTotal;

        // Assert
        result.Should().Be(expectedValor);
    }
}
