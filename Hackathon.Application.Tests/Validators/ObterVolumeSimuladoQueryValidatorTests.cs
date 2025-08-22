using FluentValidation;
using Hackathon.Application.Queries;
using Hackathon.Application.Validators;

namespace Hackathon.Application.Tests.Validators;

public class ObterVolumeSimuladoQueryValidatorTests
{
    private readonly ObterVolumeSimuladoQueryValidator _validator;

    public ObterVolumeSimuladoQueryValidatorTests()
    {
        _validator = new ObterVolumeSimuladoQueryValidator();
    }

    [Fact]
    public void Validate_ComDataValida_DeveSerValido()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComDataFutura_DeveTerErroDeValidacao()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "DataReferencia" && e.ErrorMessage == "Data de referência não pode ser futura");
    }

    [Fact]
    public void Validate_ComDataPassada_DeveSerValido()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComDataHoje_DeveSerValido()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComDataMuitoPassada_DeveSerValido()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddYears(-10)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComDataFuturaPorPouco_DeveTerErroDeValidacao()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DataReferencia");
    }

    [Fact]
    public void Validate_ComDataFuturaMuitoDistante_DeveTerErroDeValidacao()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddYears(10)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DataReferencia");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(365)]
    public void Validate_ComDatasPassadasDiferentes_DeveSerValido(int diasPassados)
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(-diasPassados)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(365)]
    public void Validate_ComDatasFuturasDiferentes_DeveTerErroDeValidacao(int diasFuturos)
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(diasFuturos)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DataReferencia");
    }

    [Fact]
    public void Validate_ComDataLimiteHoje_DeveSerValido()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComDataLimiteOntem_DeveSerValido()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComDataLimiteAmanha_DeveTerErroDeValidacao()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DataReferencia");
    }
}
