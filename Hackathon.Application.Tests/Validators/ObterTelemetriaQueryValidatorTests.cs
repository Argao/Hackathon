using FluentAssertions;
using Hackathon.Application.Queries;
using Hackathon.Application.Validators;

namespace Hackathon.Application.Tests.Validators;

public class ObterTelemetriaQueryValidatorTests
{
    private readonly ObterTelemetriaQueryValidator _validator;

    public ObterTelemetriaQueryValidatorTests()
    {
        _validator = new ObterTelemetriaQueryValidator();
    }

    [Fact]
    public void Validate_ComDataValida_DeveSerValido()
    {
        // Arrange
        var query = new ObterTelemetriaQuery(DateOnly.FromDateTime(DateTime.Today));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ComDataPassada_DeveSerValido()
    {
        // Arrange
        var query = new ObterTelemetriaQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ComDataFutura_DeveSerInvalido()
    {
        // Arrange
        var query = new ObterTelemetriaQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Contain("não pode ser futura");
    }

    [Fact]
    public void Validate_ComDataDefault_DeveSerInvalido()
    {
        // Arrange
        var query = new ObterTelemetriaQuery(default);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Contain("obrigatória");
    }
}
