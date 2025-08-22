using FluentAssertions;
using Hackathon.Application.DTOs.Responses;
using Hackathon.Domain.Enums;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class ResultadoSimulacaoDTOTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var resultado = new ResultadoSimulacaoDTO();

        // Assert
        resultado.Tipo.Should().Be((SistemaAmortizacao)0);
        resultado.Parcelas.Should().NotBeNull();
        resultado.Parcelas.Should().BeEmpty();
    }

    [Fact]
    public void Tipo_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var resultado = new ResultadoSimulacaoDTO();
        const SistemaAmortizacao expectedTipo = SistemaAmortizacao.SAC;

        // Act
        resultado.Tipo = expectedTipo;
        var result = resultado.Tipo;

        // Assert
        result.Should().Be(expectedTipo);
    }

    [Fact]
    public void Parcelas_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var resultado = new ResultadoSimulacaoDTO();
        var expectedParcelas = new List<ParcelaDTO> 
        { 
            new() { Numero = 1, ValorPrestacao = 1000m },
            new() { Numero = 2, ValorPrestacao = 1000m }
        };

        // Act
        resultado.Parcelas = expectedParcelas;
        var result = resultado.Parcelas;

        // Assert
        result.Should().BeEquivalentTo(expectedParcelas);
    }
}
