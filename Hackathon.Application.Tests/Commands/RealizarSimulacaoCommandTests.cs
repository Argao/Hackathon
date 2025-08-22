using Hackathon.Application.Commands;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Tests.Commands;

public class RealizarSimulacaoCommandTests
{
    [Fact]
    public void ToValueObjects_ComValoresValidos_DeveRetornarSucesso()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);

        // Act
        var result = command.ToValueObjects();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Valor.Should().Be(10000m);
        result.Value.Prazo.Meses.Should().Be(12);
    }

    [Theory]
    [InlineData(0, 12)]
    [InlineData(-1000, 12)]
    [InlineData(1000000000, 12)] // Valor muito alto
    public void ToValueObjects_ComValorInvalido_DeveRetornarFalha(int valor, int prazo)
    {
        // Arrange
        var command = new RealizarSimulacaoCommand((decimal)valor, prazo);

        // Act
        var result = command.ToValueObjects();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(10000, 0)]
    [InlineData(10000, -5)]
    [InlineData(10000, 601)] // Prazo muito alto
    public void ToValueObjects_ComPrazoInvalido_DeveRetornarFalha(int valor, int prazo)
    {
        // Arrange
        var command = new RealizarSimulacaoCommand((decimal)valor, prazo);

        // Act
        var result = command.ToValueObjects();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ToValueObjects_ComValorEPrazoInvalidos_DeveRetornarFalhaComPrimeiroErro()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(0m, 0);

        // Act
        var result = command.ToValueObjects();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Valor deve ser maior ou igual a R$ 0,01");
    }

    [Fact]
    public void ToValueObjects_ComValoresLimiteValidos_DeveRetornarSucesso()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(0.01m, 1); // Valores mínimos válidos

        // Act
        var result = command.ToValueObjects();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Valor.Should().Be(0.01m);
        result.Value.Prazo.Meses.Should().Be(1);
    }

    [Fact]
    public void ToValueObjects_ComValoresLimiteMaximos_DeveRetornarSucesso()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(999999999.99m, 600); // Valores máximos válidos

        // Act
        var result = command.ToValueObjects();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Valor.Should().Be(999999999.99m);
        result.Value.Prazo.Meses.Should().Be(600);
    }

    [Fact]
    public void ToValueObjects_ComValorExcedendoLimite_DeveRetornarFalha()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(1000000000m, 12); // Valor excede o limite

        // Act
        var result = command.ToValueObjects();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ToValueObjects_ComPrazoExcedendoLimite_DeveRetornarFalha()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 601); // Prazo excede o limite

        // Act
        var result = command.ToValueObjects();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }
}
