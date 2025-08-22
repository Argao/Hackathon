using FluentAssertions;
using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class SimulacaoResponseDTOTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var simulacao = new SimulacaoResponseDTO();

        // Assert
        simulacao.Id.Should().Be(Guid.Empty);
        simulacao.CodigoProduto.Should().Be(0);
        simulacao.DescricaoProduto.Should().Be(string.Empty);
        simulacao.TaxaJuros.Should().Be(0m);
        simulacao.ResultadoSimulacao.Should().NotBeNull();
        simulacao.ResultadoSimulacao.Should().BeEmpty();
    }

    [Fact]
    public void Id_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new SimulacaoResponseDTO();
        var expectedId = Guid.NewGuid();

        // Act
        simulacao.Id = expectedId;
        var result = simulacao.Id;

        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void CodigoProduto_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new SimulacaoResponseDTO();
        const int expectedCodigo = 123;

        // Act
        simulacao.CodigoProduto = expectedCodigo;
        var result = simulacao.CodigoProduto;

        // Assert
        result.Should().Be(expectedCodigo);
    }

    [Fact]
    public void DescricaoProduto_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new SimulacaoResponseDTO();
        const string expectedDescricao = "Produto Teste";

        // Act
        simulacao.DescricaoProduto = expectedDescricao;
        var result = simulacao.DescricaoProduto;

        // Assert
        result.Should().Be(expectedDescricao);
    }

    [Fact]
    public void TaxaJuros_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new SimulacaoResponseDTO();
        const decimal expectedTaxa = 0.015m;

        // Act
        simulacao.TaxaJuros = expectedTaxa;
        var result = simulacao.TaxaJuros;

        // Assert
        result.Should().Be(expectedTaxa);
    }

    [Fact]
    public void ResultadoSimulacao_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var simulacao = new SimulacaoResponseDTO();
        var expectedResultados = new List<ResultadoSimulacaoDTO> 
        { 
            new() { Tipo = Domain.Enums.SistemaAmortizacao.PRICE },
            new() { Tipo = Domain.Enums.SistemaAmortizacao.SAC }
        };

        // Act
        simulacao.ResultadoSimulacao = expectedResultados;
        var result = simulacao.ResultadoSimulacao;

        // Assert
        result.Should().BeEquivalentTo(expectedResultados);
    }
}
