using FluentAssertions;
using Hackathon.API.Contracts.Requests;

namespace Hackathon.API.Tests.Contracts.Requests;

public class SimulacaoRequestTests
{
    [Fact]
    public void SimulacaoRequest_DeveTerPropriedadesCorretas()
    {
        // Arrange & Act
        var request = new SimulacaoRequest(10000, 12);

        // Assert
        request.ValorDesejado.Should().Be(10000);
        request.Prazo.Should().Be(12);
    }

    [Fact]
    public void SimulacaoRequest_ComValoresPadrao_DeveFuncionar()
    {
        // Arrange & Act
        var request = new SimulacaoRequest(0, 0);

        // Assert
        request.ValorDesejado.Should().Be(0);
        request.Prazo.Should().Be(0);
    }

    [Fact]
    public void SimulacaoRequest_ComValoresNegativos_DeveAceitar()
    {
        // Arrange & Act
        var request = new SimulacaoRequest(-1000, -5);

        // Assert
        request.ValorDesejado.Should().Be(-1000);
        request.Prazo.Should().Be(-5);
    }

    [Fact]
    public void SimulacaoRequest_ComValoresZero_DeveAceitar()
    {
        // Arrange & Act
        var request = new SimulacaoRequest(0, 0);

        // Assert
        request.ValorDesejado.Should().Be(0);
        request.Prazo.Should().Be(0);
    }
}
