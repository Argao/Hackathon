using FluentValidation;
using Hackathon.Application.Queries;
using Hackathon.Application.Validators;

namespace Hackathon.Application.Tests.Validators;

public class ListarSimulacoesQueryValidatorTests
{
    private readonly ListarSimulacoesQueryValidator _validator;

    public ListarSimulacoesQueryValidatorTests()
    {
        _validator = new ListarSimulacoesQueryValidator();
    }

    [Fact]
    public void Validate_ComValoresValidos_DeveSerValido()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_ComNumeroPaginaInvalido_DeveTerErroDeValidacao(int numeroPagina)
    {
        // Arrange
        var query = new ListarSimulacoesQuery(numeroPagina, 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "NumeroPagina" && e.ErrorMessage == "Número da página deve ser maior ou igual a 1");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_ComTamanhoPaginaMenorQueUm_DeveTerErroDeValidacao(int tamanhoPagina)
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, tamanhoPagina);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TamanhoPagina" && e.ErrorMessage == "Tamanho da página deve ser maior ou igual a 1");
    }

    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    [InlineData(1000)]
    public void Validate_ComTamanhoPaginaMaiorQueCem_DeveTerErroDeValidacao(int tamanhoPagina)
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, tamanhoPagina);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TamanhoPagina" && e.ErrorMessage == "Tamanho da página não pode exceder 100 itens");
    }

    [Fact]
    public void Validate_ComNumeroPaginaETamanhoPaginaInvalidos_DeveTerErrosParaAmbos()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(0, 0);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.PropertyName == "NumeroPagina");
        result.Errors.Should().Contain(e => e.PropertyName == "TamanhoPagina");
    }

    [Fact]
    public void Validate_ComValoresLimiteValidos_DeveSerValido()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 1);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComValoresLimiteMaximos_DeveSerValido()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 100);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_ComTamanhoPaginaValido_DeveSerValido(int tamanhoPagina)
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, tamanhoPagina);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Validate_ComNumeroPaginaValido_DeveSerValido(int numeroPagina)
    {
        // Arrange
        var query = new ListarSimulacoesQuery(numeroPagina, 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComTamanhoPaginaExcedendoLimitePorPouco_DeveTerErroDeValidacao()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 101);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TamanhoPagina");
    }

    [Fact]
    public void Validate_ComNumeroPaginaExcedendoLimitePorPouco_DeveTerErroDeValidacao()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(0, 10);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NumeroPagina");
    }

    [Fact]
    public void Validate_ComValoresMediosValidos_DeveSerValido()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(5, 25);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComValoresGrandesValidos_DeveSerValido()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1000, 100);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
