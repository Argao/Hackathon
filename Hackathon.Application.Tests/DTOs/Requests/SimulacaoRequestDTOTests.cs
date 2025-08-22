using FluentAssertions;
using Hackathon.Application.DTOs.Requests;

namespace Hackathon.Application.Tests.DTOs.Requests;

public class SimulacaoRequestDTOTests
{
    [Fact]
    public void Valor_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var request = new SimulacaoRequestDTO();
        const decimal expectedValor = 50000.00m;

        // Act
        request.Valor = expectedValor;
        var result = request.Valor;

        // Assert
        result.Should().Be(expectedValor);
    }

    [Fact]
    public void Prazo_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var request = new SimulacaoRequestDTO();
        const int expectedPrazo = 24;

        // Act
        request.Prazo = expectedPrazo;
        var result = request.Prazo;

        // Assert
        result.Should().Be(expectedPrazo);
    }
}
