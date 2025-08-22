using FluentAssertions;
using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class VolumeSimuladoResponseDTOTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var response = new VolumeSimuladoResponseDTO();

        // Assert
        response.DataReferencia.Should().Be(string.Empty);
        response.Simulacoes.Should().NotBeNull();
        response.Simulacoes.Should().BeEmpty();
    }

    [Fact]
    public void DataReferencia_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var response = new VolumeSimuladoResponseDTO();
        const string expectedData = "2024-01-15";

        // Act
        response.DataReferencia = expectedData;
        var result = response.DataReferencia;

        // Assert
        result.Should().Be(expectedData);
    }

    [Fact]
    public void Simulacoes_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var response = new VolumeSimuladoResponseDTO();
        var expectedSimulacoes = new List<VolumeSimuladoProdutoDTO> 
        { 
            new() { CodigoProduto = 1, DescricaoProduto = "Produto 1" },
            new() { CodigoProduto = 2, DescricaoProduto = "Produto 2" }
        };

        // Act
        response.Simulacoes = expectedSimulacoes;
        var result = response.Simulacoes;

        // Assert
        result.Should().BeEquivalentTo(expectedSimulacoes);
    }
}
