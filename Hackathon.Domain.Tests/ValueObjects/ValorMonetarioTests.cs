using FluentAssertions;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.ValueObjects;

public class ValorMonetarioTests
{
    [Theory]
    [InlineData(0.00)]
    [InlineData(100.50)]
    [InlineData(999_999_999_999.99)]
    public void Create_ComValorValido_DeveRetornarSucesso(decimal valor)
    {
        // Act
        var result = ValorMonetario.Create(valor);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(-1.00)]
    [InlineData(-100.50)]
    public void Create_ComValorNegativo_DeveRetornarFalha(decimal valor)
    {
        // Act
        var result = ValorMonetario.Create(valor);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("deve ser maior ou igual a R$ 0,00");
    }

    [Theory]
    [InlineData(1_000_000_000_000.00)]
    [InlineData(2_000_000_000_000.00)]
    public void Create_ComValorAcimaDoMaximo_DeveRetornarFalha(decimal valor)
    {
        // Act
        var result = ValorMonetario.Create(valor);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("não pode exceder R$ 999.999.999.999,99");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(100.50)]
    [InlineData(999_999_999_999.99)]
    public void CreatePositivo_ComValorPositivo_DeveRetornarSucesso(decimal valor)
    {
        // Act
        var result = ValorMonetario.CreatePositivo(valor);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    [InlineData(-100.50)]
    public void CreatePositivo_ComValorZeroOuNegativo_DeveRetornarFalha(decimal valor)
    {
        // Act
        var result = ValorMonetario.CreatePositivo(valor);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Valor deve ser maior que zero");
    }

    [Fact]
    public void Zero_DeveRetornarValorZero()
    {
        // Act
        var zero = ValorMonetario.Zero;

        // Assert
        zero.Valor.Should().Be(0.00m);
    }

    [Fact]
    public void ConversaoImplicita_ParaDecimal_DeveFuncionar()
    {
        // Arrange
        var valorMonetario = ValorMonetario.Create(100.50m).Value;

        // Act
        decimal valor = valorMonetario;

        // Assert
        valor.Should().Be(100.50m);
    }

    [Theory]
    [InlineData(100.00, 50.00, 150.00)]
    [InlineData(0.00, 0.00, 0.00)]
    [InlineData(999.99, 0.01, 1000.00)]
    public void OperadorSoma_DeveCalcularCorretamente(decimal a, decimal b, decimal esperado)
    {
        // Arrange
        var valorA = ValorMonetario.Create(a).Value;
        var valorB = ValorMonetario.Create(b).Value;

        // Act
        var resultado = valorA + valorB;

        // Assert
        resultado.Valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100.00, 50.00, 50.00)]
    [InlineData(0.00, 0.00, 0.00)]
    [InlineData(1000.00, 999.99, 0.01)]
    public void OperadorSubtracao_DeveCalcularCorretamente(decimal a, decimal b, decimal esperado)
    {
        // Arrange
        var valorA = ValorMonetario.Create(a).Value;
        var valorB = ValorMonetario.Create(b).Value;

        // Act
        var resultado = valorA - valorB;

        // Assert
        resultado.Valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100.00, 2.5, 250.00)]
    [InlineData(0.00, 10.00, 0.00)]
    [InlineData(50.00, 0.5, 25.00)]
    public void OperadorMultiplicacao_ComDecimal_DeveCalcularCorretamente(decimal valor, decimal multiplicador, decimal esperado)
    {
        // Arrange
        var valorMonetario = ValorMonetario.Create(valor).Value;

        // Act
        var resultado = valorMonetario * multiplicador;

        // Assert
        resultado.Valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100.00, 2, 200.00)]
    [InlineData(0.00, 10, 0.00)]
    [InlineData(50.00, 0, 0.00)]
    public void OperadorMultiplicacao_ComInteiro_DeveCalcularCorretamente(decimal valor, int multiplicador, decimal esperado)
    {
        // Arrange
        var valorMonetario = ValorMonetario.Create(valor).Value;

        // Act
        var resultado = valorMonetario * multiplicador;

        // Assert
        resultado.Valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100.00, 2.0, 50.00)]
    [InlineData(0.00, 10.00, 0.00)]
    [InlineData(50.00, 0.5, 100.00)]
    public void OperadorDivisao_DeveCalcularCorretamente(decimal valor, decimal divisor, decimal esperado)
    {
        // Arrange
        var valorMonetario = ValorMonetario.Create(valor).Value;

        // Act
        var resultado = valorMonetario / divisor;

        // Assert
        resultado.Valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100.00, 50.00, true)]
    [InlineData(50.00, 100.00, false)]
    [InlineData(100.00, 100.00, false)]
    public void OperadorMaiorQue_DeveCompararCorretamente(decimal a, decimal b, bool esperado)
    {
        // Arrange
        var valorA = ValorMonetario.Create(a).Value;
        var valorB = ValorMonetario.Create(b).Value;

        // Act
        var resultado = valorA > valorB;

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData(50.00, 100.00, true)]
    [InlineData(100.00, 50.00, false)]
    [InlineData(100.00, 100.00, false)]
    public void OperadorMenorQue_DeveCompararCorretamente(decimal a, decimal b, bool esperado)
    {
        // Arrange
        var valorA = ValorMonetario.Create(a).Value;
        var valorB = ValorMonetario.Create(b).Value;

        // Act
        var resultado = valorA < valorB;

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100.00, 50.00, true)]
    [InlineData(50.00, 100.00, false)]
    [InlineData(100.00, 100.00, true)]
    public void OperadorMaiorOuIgual_DeveCompararCorretamente(decimal a, decimal b, bool esperado)
    {
        // Arrange
        var valorA = ValorMonetario.Create(a).Value;
        var valorB = ValorMonetario.Create(b).Value;

        // Act
        var resultado = valorA >= valorB;

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData(50.00, 100.00, true)]
    [InlineData(100.00, 50.00, false)]
    [InlineData(100.00, 100.00, true)]
    public void OperadorMenorOuIgual_DeveCompararCorretamente(decimal a, decimal b, bool esperado)
    {
        // Arrange
        var valorA = ValorMonetario.Create(a).Value;
        var valorB = ValorMonetario.Create(b).Value;

        // Act
        var resultado = valorA <= valorB;

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100.123, 100.12)]
    [InlineData(100.125, 100.13)]
    [InlineData(100.126, 100.13)]
    [InlineData(100.00, 100.00)]
    public void ArredondarFinanceiro_DeveArredondarCorretamente(decimal valor, decimal esperado)
    {
        // Arrange
        var valorMonetario = ValorMonetario.Create(valor).Value;

        // Act
        var resultado = valorMonetario.ArredondarFinanceiro();

        // Assert
        resultado.Valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100.50, "R$ 100,50")]
    [InlineData(0.00, "R$ 0,00")]
    [InlineData(1000.00, "R$ 1.000,00")]
    public void ToString_DeveFormatarCorretamente(decimal valor, string esperado)
    {
        // Arrange
        var valorMonetario = ValorMonetario.Create(valor).Value;

        // Act
        var resultado = valorMonetario.ToString();

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void RecordStruct_DeveTerIgualdadePorValor()
    {
        // Arrange
        var valor1 = ValorMonetario.Create(100.50m).Value;
        var valor2 = ValorMonetario.Create(100.50m).Value;
        var valor3 = ValorMonetario.Create(200.00m).Value;

        // Act & Assert
        valor1.Should().Be(valor2);
        valor1.Should().NotBe(valor3);
    }
}
