using FluentValidation;
using Hackathon.Application.Commands;
using Hackathon.Application.Validators;

namespace Hackathon.Application.Tests.Validators;

public class RealizarSimulacaoCommandValidatorTests
{
    private readonly RealizarSimulacaoCommandValidator _validator;

    public RealizarSimulacaoCommandValidatorTests()
    {
        _validator = new RealizarSimulacaoCommandValidator();
    }

    [Fact]
    public void Validate_ComValoresValidos_DeveSerValido()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    [InlineData(-1)]
    public void Validate_ComValorInvalido_DeveTerErroDeValidacao(int valor)
    {
        // Arrange
        var command = new RealizarSimulacaoCommand((decimal)valor, 12);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Valor" && e.ErrorMessage == "Valor deve ser maior que zero");
    }

    [Theory]
    [InlineData(1000000000)]
    public void Validate_ComValorExcedendoLimite_DeveTerErroDeValidacao(int valor)
    {
        // Arrange
        var command = new RealizarSimulacaoCommand((decimal)valor, 12);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Valor");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_ComPrazoInvalido_DeveTerErroDeValidacao(int prazo)
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, prazo);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Prazo" && e.ErrorMessage == "Prazo deve ser maior que zero");
    }

    [Theory]
    [InlineData(601)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void Validate_ComPrazoExcedendoLimite_DeveTerErroDeValidacao(int prazo)
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, prazo);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Prazo" && e.ErrorMessage == "Prazo não pode exceder 600 meses (50 anos)");
    }

    [Fact]
    public void Validate_ComValorEPrazoInvalidos_DeveTerErrosParaAmbos()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(-1000m, -5);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.PropertyName == "Valor");
        result.Errors.Should().Contain(e => e.PropertyName == "Prazo");
    }

    [Fact]
    public void Validate_ComValoresLimiteValidos_DeveSerValido()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(0.01m, 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComValoresLimiteMaximos_DeveSerValido()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(999999999.99m, 600);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComValorExcedendoLimitePorPouco_DeveTerErroDeValidacao()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(1000000000m, 12);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Valor");
    }

    [Fact]
    public void Validate_ComPrazoExcedendoLimitePorPouco_DeveTerErroDeValidacao()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 601);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Prazo");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(999999999)]
    public void Validate_ComValoresValidosDiferentes_DeveSerValido(int valor)
    {
        // Arrange
        var command = new RealizarSimulacaoCommand((decimal)valor, 12);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    [InlineData(60)]
    [InlineData(120)]
    [InlineData(300)]
    [InlineData(600)]
    public void Validate_ComPrazosValidosDiferentes_DeveSerValido(int prazo)
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, prazo);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
