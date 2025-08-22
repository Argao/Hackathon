using FluentAssertions;
using Hackathon.Domain.Enums;
using Xunit;

namespace Hackathon.Domain.Tests.Enums;

public class SistemaAmortizacaoTests
{
    [Fact]
    public void SistemaAmortizacao_DeveTerValoresCorretos()
    {
        // Act & Assert
        ((int)SistemaAmortizacao.SAC).Should().Be(1);
        ((int)SistemaAmortizacao.PRICE).Should().Be(2);
    }

    [Fact]
    public void SistemaAmortizacao_DeveTerDoisValores()
    {
        // Act
        var valores = Enum.GetValues<SistemaAmortizacao>();

        // Assert
        valores.Should().HaveCount(2);
        valores.Should().Contain(SistemaAmortizacao.SAC);
        valores.Should().Contain(SistemaAmortizacao.PRICE);
    }

    [Theory]
    [InlineData(SistemaAmortizacao.SAC, 1)]
    [InlineData(SistemaAmortizacao.PRICE, 2)]
    public void ConversaoParaInt_DeveRetornarValorCorreto(SistemaAmortizacao sistema, int valorEsperado)
    {
        // Act
        var valor = (int)sistema;

        // Assert
        valor.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData(1, SistemaAmortizacao.SAC)]
    [InlineData(2, SistemaAmortizacao.PRICE)]
    public void ConversaoDeInt_DeveRetornarEnumCorreto(int valor, SistemaAmortizacao sistemaEsperado)
    {
        // Act
        var sistema = (SistemaAmortizacao)valor;

        // Assert
        sistema.Should().Be(sistemaEsperado);
    }

    [Fact]
    public void SistemaAmortizacao_DeveSerEnumPublico()
    {
        // Act & Assert
        typeof(SistemaAmortizacao).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void SistemaAmortizacao_DeveSerTipoEnum()
    {
        // Act & Assert
        typeof(SistemaAmortizacao).IsEnum.Should().BeTrue();
    }

    [Fact]
    public void SistemaAmortizacao_DeveTerNomesCorretos()
    {
        // Act & Assert
        SistemaAmortizacao.SAC.ToString().Should().Be("SAC");
        SistemaAmortizacao.PRICE.ToString().Should().Be("PRICE");
    }

    [Theory]
    [InlineData("SAC", SistemaAmortizacao.SAC)]
    [InlineData("PRICE", SistemaAmortizacao.PRICE)]
    public void Parse_DeveConverterStringCorretamente(string nome, SistemaAmortizacao sistemaEsperado)
    {
        // Act
        var sistema = Enum.Parse<SistemaAmortizacao>(nome);

        // Assert
        sistema.Should().Be(sistemaEsperado);
    }

    [Theory]
    [InlineData("sac", SistemaAmortizacao.SAC)]
    [InlineData("price", SistemaAmortizacao.PRICE)]
    public void Parse_ComIgnoreCase_DeveConverterStringCorretamente(string nome, SistemaAmortizacao sistemaEsperado)
    {
        // Act
        var sistema = Enum.Parse<SistemaAmortizacao>(nome, ignoreCase: true);

        // Assert
        sistema.Should().Be(sistemaEsperado);
    }

    [Fact]
    public void TryParse_ComStringValida_DeveRetornarTrue()
    {
        // Act
        var sucesso = Enum.TryParse<SistemaAmortizacao>("SAC", out var sistema);

        // Assert
        sucesso.Should().BeTrue();
        sistema.Should().Be(SistemaAmortizacao.SAC);
    }

    [Fact]
    public void TryParse_ComStringInvalida_DeveRetornarFalse()
    {
        // Act
        var sucesso = Enum.TryParse<SistemaAmortizacao>("INVALIDO", out var sistema);

        // Assert
        sucesso.Should().BeFalse();
        sistema.Should().Be(default(SistemaAmortizacao));
    }

    [Fact]
    public void Comparacao_DeveFuncionarCorretamente()
    {
        // Act & Assert
        (SistemaAmortizacao.SAC == SistemaAmortizacao.SAC).Should().BeTrue();
        (SistemaAmortizacao.SAC != SistemaAmortizacao.PRICE).Should().BeTrue();
        (SistemaAmortizacao.SAC < SistemaAmortizacao.PRICE).Should().BeTrue();
        (SistemaAmortizacao.PRICE > SistemaAmortizacao.SAC).Should().BeTrue();
        (SistemaAmortizacao.SAC <= SistemaAmortizacao.PRICE).Should().BeTrue();
        (SistemaAmortizacao.PRICE >= SistemaAmortizacao.SAC).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_DeveRetornarValoresDiferentes()
    {
        // Act
        var hashSAC = SistemaAmortizacao.SAC.GetHashCode();
        var hashPRICE = SistemaAmortizacao.PRICE.GetHashCode();

        // Assert
        hashSAC.Should().NotBe(hashPRICE);
    }

    [Fact]
    public void Equals_DeveCompararCorretamente()
    {
        // Act & Assert
        SistemaAmortizacao.SAC.Equals(SistemaAmortizacao.SAC).Should().BeTrue();
        SistemaAmortizacao.SAC.Equals(SistemaAmortizacao.PRICE).Should().BeFalse();
        SistemaAmortizacao.SAC.Equals((object)SistemaAmortizacao.SAC).Should().BeTrue();
        SistemaAmortizacao.SAC.Equals((object)SistemaAmortizacao.PRICE).Should().BeFalse();
    }

    [Fact]
    public void GetName_DeveRetornarNomeCorreto()
    {
        // Act & Assert
        Enum.GetName(SistemaAmortizacao.SAC).Should().Be("SAC");
        Enum.GetName(SistemaAmortizacao.PRICE).Should().Be("PRICE");
    }

    [Fact]
    public void GetNames_DeveRetornarTodosOsNomes()
    {
        // Act
        var nomes = Enum.GetNames<SistemaAmortizacao>();

        // Assert
        nomes.Should().HaveCount(2);
        nomes.Should().Contain("SAC");
        nomes.Should().Contain("PRICE");
    }
}
