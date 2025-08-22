using Hackathon.Application.Queries;

namespace Hackathon.Application.Tests.Queries;

public class ListarSimulacoesQueryTests
{
    [Fact]
    public void GetValidPageNumber_ComNumeroPaginaValido_DeveRetornarMesmoValor()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(5, 10);

        // Act
        var result = query.GetValidPageNumber();

        // Assert
        result.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void GetValidPageNumber_ComNumeroPaginaInvalido_DeveRetornarUm(int numeroPagina)
    {
        // Arrange
        var query = new ListarSimulacoesQuery(numeroPagina, 10);

        // Act
        var result = query.GetValidPageNumber();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void GetValidPageSize_ComTamanhoPaginaValido_DeveRetornarMesmoValor()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 50);

        // Act
        var result = query.GetValidPageSize();

        // Assert
        result.Should().Be(50);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void GetValidPageSize_ComTamanhoPaginaMenorQueUm_DeveRetornarUm(int tamanhoPagina)
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, tamanhoPagina);

        // Act
        var result = query.GetValidPageSize();

        // Assert
        result.Should().Be(1);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    [InlineData(1000)]
    public void GetValidPageSize_ComTamanhoPaginaMaiorQueCem_DeveRetornarCem(int tamanhoPagina)
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, tamanhoPagina);

        // Act
        var result = query.GetValidPageSize();

        // Assert
        result.Should().Be(100);
    }

    [Fact]
    public void GetValidPageSize_ComTamanhoPaginaNoLimite_DeveRetornarCem()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 100);

        // Act
        var result = query.GetValidPageSize();

        // Assert
        result.Should().Be(100);
    }

    [Fact]
    public void GetValidPageSize_ComTamanhoPaginaUm_DeveRetornarUm()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 1);

        // Act
        var result = query.GetValidPageSize();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void GetValidPageNumber_ComNumeroPaginaUm_DeveRetornarUm()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 10);

        // Act
        var result = query.GetValidPageNumber();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void GetValidPageNumber_ComNumeroPaginaGrande_DeveRetornarMesmoValor()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1000, 10);

        // Act
        var result = query.GetValidPageNumber();

        // Assert
        result.Should().Be(1000);
    }

    [Fact]
    public void GetValidPageSize_ComTamanhoPaginaMedio_DeveRetornarMesmoValor()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 25);

        // Act
        var result = query.GetValidPageSize();

        // Assert
        result.Should().Be(25);
    }
}
