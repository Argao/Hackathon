using FluentAssertions;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.ValueObjects;

public class ValorEmprestimoTests
{
    [Theory]
    [InlineData(0.01)]
    [InlineData(100.50)]
    [InlineData(999_999_999.99)]
    public void Create_ComValorValido_DeveRetornarSucesso(decimal valor)
    {
        // Act
        var result = ValorEmprestimo.Create(valor);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    [InlineData(-100.50)]
    public void Create_ComValorMenorQueMinimo_DeveRetornarFalha(decimal valor)
    {
        // Act
        var result = ValorEmprestimo.Create(valor);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("deve ser maior ou igual a R$ 0,01");
    }

    [Theory]
    [InlineData(1_000_000_000.00)]
    [InlineData(2_000_000_000.00)]
    public void Create_ComValorAcimaDoMaximo_DeveRetornarFalha(decimal valor)
    {
        // Act
        var result = ValorEmprestimo.Create(valor);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("não pode exceder R$ 999.999.999,99");
    }

    [Fact]
    public void ConversaoImplicita_ParaDecimal_DeveFuncionar()
    {
        // Arrange
        var valorEmprestimo = ValorEmprestimo.Create(100.50m).Value;

        // Act
        decimal valor = valorEmprestimo;

        // Assert
        valor.Should().Be(100.50m);
    }

    [Theory]
    [InlineData(100.50, "R$ 100,50")]
    [InlineData(0.01, "R$ 0,01")]
    [InlineData(1000.00, "R$ 1.000,00")]
    public void ToString_DeveFormatarCorretamente(decimal valor, string esperado)
    {
        // Arrange
        var valorEmprestimo = ValorEmprestimo.Create(valor).Value;

        // Act
        var resultado = valorEmprestimo.ToString();

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void RecordStruct_DeveTerIgualdadePorValor()
    {
        // Arrange
        var valor1 = ValorEmprestimo.Create(100.50m).Value;
        var valor2 = ValorEmprestimo.Create(100.50m).Value;
        var valor3 = ValorEmprestimo.Create(200.00m).Value;

        // Act & Assert
        valor1.Should().Be(valor2);
        valor1.Should().NotBe(valor3);
    }
}
