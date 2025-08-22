using FluentAssertions;
using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class VolumeSimuladoProdutoDTOTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var produto = new VolumeSimuladoProdutoDTO();

        // Assert
        produto.CodigoProduto.Should().Be(0);
        produto.DescricaoProduto.Should().Be(string.Empty);
        produto.TaxaMediaJuro.Should().Be(0m);
        produto.ValorMedioPrestacao.Should().Be(0m);
        produto.ValorTotalDesejado.Should().Be(0m);
        produto.ValorTotalCredito.Should().Be(0m);
    }

    [Fact]
    public void CodigoProduto_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var produto = new VolumeSimuladoProdutoDTO();
        const int expectedCodigo = 123;

        // Act
        produto.CodigoProduto = expectedCodigo;
        var result = produto.CodigoProduto;

        // Assert
        result.Should().Be(expectedCodigo);
    }

    [Fact]
    public void DescricaoProduto_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var produto = new VolumeSimuladoProdutoDTO();
        const string expectedDescricao = "Produto Teste";

        // Act
        produto.DescricaoProduto = expectedDescricao;
        var result = produto.DescricaoProduto;

        // Assert
        result.Should().Be(expectedDescricao);
    }

    [Fact]
    public void TaxaMediaJuro_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var produto = new VolumeSimuladoProdutoDTO();
        const decimal expectedTaxa = 0.015m;

        // Act
        produto.TaxaMediaJuro = expectedTaxa;
        var result = produto.TaxaMediaJuro;

        // Assert
        result.Should().Be(expectedTaxa);
    }

    [Fact]
    public void ValorMedioPrestacao_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var produto = new VolumeSimuladoProdutoDTO();
        const decimal expectedValor = 2500.00m;

        // Act
        produto.ValorMedioPrestacao = expectedValor;
        var result = produto.ValorMedioPrestacao;

        // Assert
        result.Should().Be(expectedValor);
    }

    [Fact]
    public void ValorTotalDesejado_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var produto = new VolumeSimuladoProdutoDTO();
        const decimal expectedValor = 100000.00m;

        // Act
        produto.ValorTotalDesejado = expectedValor;
        var result = produto.ValorTotalDesejado;

        // Assert
        result.Should().Be(expectedValor);
    }

    [Fact]
    public void ValorTotalCredito_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var produto = new VolumeSimuladoProdutoDTO();
        const decimal expectedValor = 120000.00m;

        // Act
        produto.ValorTotalCredito = expectedValor;
        var result = produto.ValorTotalCredito;

        // Assert
        result.Should().Be(expectedValor);
    }
}
