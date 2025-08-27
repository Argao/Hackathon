using Hackathon.Application.Queries;

namespace Hackathon.Application.Tests.Queries;

public class ObterTelemetriaQueryTests
{
    [Fact]
    public void ObterTelemetriaQuery_Constructor_DeveInicializarCorretamente()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var query = new ObterTelemetriaQuery(dataReferencia);

        // Assert
        query.DataReferencia.Should().Be(dataReferencia);
    }

    [Fact]
    public void IsValid_ComDataPassada_DeveRetornarTrue()
    {
        // Arrange
        var dataPassada = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var query = new ObterTelemetriaQuery(dataPassada);

        // Act
        var isValid = query.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ComDataAtual_DeveRetornarTrue()
    {
        // Arrange
        var dataAtual = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterTelemetriaQuery(dataAtual);

        // Act
        var isValid = query.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ComDataFutura_DeveRetornarFalse()
    {
        // Arrange
        var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var query = new ObterTelemetriaQuery(dataFutura);

        // Act
        var isValid = query.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void GetValidDataReferencia_ComDataValida_DeveRetornarData()
    {
        // Arrange
        var dataValida = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterTelemetriaQuery(dataValida);

        // Act
        var result = query.GetValidDataReferencia();

        // Assert
        result.Should().Be(dataValida);
    }

    [Fact]
    public void GetValidDataReferencia_ComDataFutura_DeveLancarArgumentException()
    {
        // Arrange
        var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var query = new ObterTelemetriaQuery(dataFutura);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => query.GetValidDataReferencia());
        exception.Message.Should().Contain("não pode ser futura");
    }
}
