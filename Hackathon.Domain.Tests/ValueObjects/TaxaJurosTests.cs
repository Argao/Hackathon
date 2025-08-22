using FluentAssertions;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.ValueObjects;

public class TaxaJurosTests
{
    [Theory]
    [InlineData(0.000001)]
    [InlineData(0.015)]
    [InlineData(0.50)]
    public void Create_ComTaxaValida_DeveRetornarSucesso(decimal taxa)
    {
        // Act
        var result = TaxaJuros.Create(taxa);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Taxa.Should().Be(taxa);
    }

    [Theory]
    [InlineData(0.0000009)]
    [InlineData(0.0000001)]
    [InlineData(-0.001)]
    public void Create_ComTaxaMuitoBaixa_DeveRetornarFalha(decimal taxa)
    {
        // Act
        var result = TaxaJuros.Create(taxa);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("deve ser maior ou igual a 0,000100%");
    }

    [Theory]
    [InlineData(0.51)]
    [InlineData(1.00)]
    [InlineData(2.00)]
    public void Create_ComTaxaAcimaDoMaximo_DeveRetornarFalha(decimal taxa)
    {
        // Act
        var result = TaxaJuros.Create(taxa);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("não pode exceder 50,00% ao mês");
    }

    [Theory]
    [InlineData(1.5, 0.015)]
    [InlineData(0.1, 0.001)]
    [InlineData(50.0, 0.50)]
    public void CreateFromPercentual_ComPercentualValido_DeveRetornarSucesso(decimal percentual, decimal taxaEsperada)
    {
        // Act
        var result = TaxaJuros.CreateFromPercentual(percentual);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Taxa.Should().Be(taxaEsperada);
    }

    [Theory]
    [InlineData(-1.0)]
    public void CreateFromPercentual_ComPercentualInvalido_DeveRetornarFalha(decimal percentual)
    {
        // Act
        var result = TaxaJuros.CreateFromPercentual(percentual);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Theory]
    [InlineData(0.015, 1.5)]
    [InlineData(0.001, 0.1)]
    [InlineData(0.50, 50.0)]
    public void ParaPercentual_DeveConverterCorretamente(decimal taxa, decimal percentualEsperado)
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(taxa).Value;

        // Act
        var percentual = taxaJuros.ParaPercentual();

        // Assert
        percentual.Should().Be(percentualEsperado);
    }

    [Fact]
    public void ConversaoImplicita_ParaDecimal_DeveFuncionar()
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(0.015m).Value;

        // Act
        decimal taxa = taxaJuros;

        // Assert
        taxa.Should().Be(0.015m);
    }

    [Theory]
    [InlineData(0.01, 0.005, 0.015)]
    [InlineData(0.10, 0.20, 0.30)]
    [InlineData(0.001, 0.002, 0.003)]
    public void OperadorSoma_DeveCalcularCorretamente(decimal a, decimal b, decimal esperado)
    {
        // Arrange
        var taxaA = TaxaJuros.Create(a).Value;
        var taxaB = TaxaJuros.Create(b).Value;

        // Act
        var resultado = taxaA + taxaB;

        // Assert
        resultado.Taxa.Should().Be(esperado);
    }

    [Theory]
    [InlineData(0.015, 0.005, 0.010)]
    [InlineData(0.30, 0.20, 0.10)]
    [InlineData(0.003, 0.002, 0.001)]
    public void OperadorSubtracao_DeveCalcularCorretamente(decimal a, decimal b, decimal esperado)
    {
        // Arrange
        var taxaA = TaxaJuros.Create(a).Value;
        var taxaB = TaxaJuros.Create(b).Value;

        // Act
        var resultado = taxaA - taxaB;

        // Assert
        resultado.Taxa.Should().Be(esperado);
    }

    [Theory]
    [InlineData(0.01, 2.5, 0.025)]
    [InlineData(0.10, 0.5, 0.05)]
    [InlineData(0.001, 10, 0.01)]
    public void OperadorMultiplicacao_DeveCalcularCorretamente(decimal taxa, decimal multiplicador, decimal esperado)
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(taxa).Value;

        // Act
        var resultado = taxaJuros * multiplicador;

        // Assert
        resultado.Taxa.Should().Be(esperado);
    }

    [Theory]
    [InlineData(0.015, "1,5000%")]
    [InlineData(0.001, "0,1000%")]
    [InlineData(0.50, "50,0000%")]
    public void ToString_DeveFormatarCorretamente(decimal taxa, string esperado)
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(taxa).Value;

        // Act
        var resultado = taxaJuros.ToString();

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void RecordStruct_DeveTerIgualdadePorValor()
    {
        // Arrange
        var taxa1 = TaxaJuros.Create(0.015m).Value;
        var taxa2 = TaxaJuros.Create(0.015m).Value;
        var taxa3 = TaxaJuros.Create(0.020m).Value;

        // Act & Assert
        taxa1.Should().Be(taxa2);
        taxa1.Should().NotBe(taxa3);
    }
}
