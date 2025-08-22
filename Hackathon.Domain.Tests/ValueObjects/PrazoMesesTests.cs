using FluentAssertions;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.ValueObjects;

public class PrazoMesesTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    [InlineData(360)]
    [InlineData(600)]
    public void Create_ComPrazoValido_DeveRetornarSucesso(int meses)
    {
        // Act
        var result = PrazoMeses.Create(meses);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Meses.Should().Be(meses);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-12)]
    public void Create_ComPrazoMenorQueMinimo_DeveRetornarFalha(int meses)
    {
        // Act
        var result = PrazoMeses.Create(meses);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("deve ser maior ou igual a 1 mês");
    }

    [Theory]
    [InlineData(601)]
    [InlineData(720)]
    [InlineData(1000)]
    public void Create_ComPrazoAcimaDoMaximo_DeveRetornarFalha(int meses)
    {
        // Act
        var result = PrazoMeses.Create(meses);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("não pode exceder 600 meses (50 anos)");
    }

    [Fact]
    public void ConversaoImplicita_ParaInt_DeveFuncionar()
    {
        // Arrange
        var prazo = PrazoMeses.Create(12).Value;

        // Act
        int meses = prazo;

        // Assert
        meses.Should().Be(12);
    }

    [Fact]
    public void ConversaoImplicita_ParaShort_DeveFuncionar()
    {
        // Arrange
        var prazo = PrazoMeses.Create(12).Value;

        // Act
        short meses = prazo;

        // Assert
        meses.Should().Be(12);
    }

    [Theory]
    [InlineData(1, "1 mês")]
    [InlineData(12, "12 meses")]
    [InlineData(360, "360 meses")]
    public void ToString_DeveFormatarCorretamente(int meses, string esperado)
    {
        // Arrange
        var prazo = PrazoMeses.Create(meses).Value;

        // Act
        var resultado = prazo.ToString();

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void RecordStruct_DeveTerIgualdadePorValor()
    {
        // Arrange
        var prazo1 = PrazoMeses.Create(12).Value;
        var prazo2 = PrazoMeses.Create(12).Value;
        var prazo3 = PrazoMeses.Create(24).Value;

        // Act & Assert
        prazo1.Should().Be(prazo2);
        prazo1.Should().NotBe(prazo3);
    }
}
