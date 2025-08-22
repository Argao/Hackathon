using FluentAssertions;
using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class ListarSimulacoesDTOTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var simulacao = new ListarSimulacoesDTO();

        // Assert
        simulacao.Id.Should().Be(Guid.Empty);
        simulacao.ValorDesejado.Should().Be(0m);
        simulacao.PrazoMeses.Should().Be(0);
        simulacao.ResultadoSimulacao.Should().NotBeNull();
        simulacao.ResultadoSimulacao.Should().BeEmpty();
    }

    [Fact]
    public void Id_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new ListarSimulacoesDTO();
        var expectedId = Guid.NewGuid();

        // Act
        simulacao.Id = expectedId;
        var result = simulacao.Id;

        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void ValorDesejado_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new ListarSimulacoesDTO();
        const decimal expectedValor = 50000.00m;

        // Act
        simulacao.ValorDesejado = expectedValor;
        var result = simulacao.ValorDesejado;

        // Assert
        result.Should().Be(expectedValor);
    }

    [Fact]
    public void PrazoMeses_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new ListarSimulacoesDTO();
        const short expectedPrazo = 24;

        // Act
        simulacao.PrazoMeses = expectedPrazo;
        var result = simulacao.PrazoMeses;

        // Assert
        result.Should().Be(expectedPrazo);
    }

    [Fact]
    public void ResultadoSimulacao_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new ListarSimulacoesDTO();
        var expectedResultados = new List<ValorTotalParcelasDTO> 
        { 
            new() { Tipo = Domain.Enums.SistemaAmortizacao.PRICE, ValorTotal = 60000m },
            new() { Tipo = Domain.Enums.SistemaAmortizacao.SAC, ValorTotal = 58000m }
        };

        // Act
        simulacao.ResultadoSimulacao = expectedResultados;
        var result = simulacao.ResultadoSimulacao;

        // Assert
        result.Should().BeEquivalentTo(expectedResultados);
    }
}
