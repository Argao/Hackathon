using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.Interfaces.Services;

public class ICalculadoraAmortizacaoTests
{
    [Fact]
    public void ICalculadoraAmortizacao_DeveSerInterface()
    {
        // Act & Assert
        typeof(ICalculadoraAmortizacao).IsInterface.Should().BeTrue();
    }

    [Fact]
    public void ICalculadoraAmortizacao_DeveSerPublica()
    {
        // Act & Assert
        typeof(ICalculadoraAmortizacao).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void ICalculadoraAmortizacao_DeveTerPropriedadeTipo()
    {
        // Arrange
        var tipo = typeof(ICalculadoraAmortizacao);

        // Act
        var propriedade = tipo.GetProperty("Tipo");

        // Assert
        propriedade.Should().NotBeNull();
        propriedade!.PropertyType.Should().Be(typeof(SistemaAmortizacao));
        propriedade.CanRead.Should().BeTrue();
        propriedade.CanWrite.Should().BeFalse();
    }

    [Fact]
    public void ICalculadoraAmortizacao_DeveTerMetodoCalcular()
    {
        // Arrange
        var tipo = typeof(ICalculadoraAmortizacao);

        // Act
        var metodo = tipo.GetMethod("Calcular");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(ResultadoSimulacao));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void ICalculadoraAmortizacao_DeveTerParametrosCorretosNoMetodoCalcular()
    {
        // Arrange
        var tipo = typeof(ICalculadoraAmortizacao);
        var metodo = tipo.GetMethod("Calcular");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(3);
        parametros[0].ParameterType.Should().Be(typeof(ValorMonetario));
        parametros[0].Name.Should().Be("valorPrincipal");
        parametros[1].ParameterType.Should().Be(typeof(TaxaJuros));
        parametros[1].Name.Should().Be("taxaMensal");
        parametros[2].ParameterType.Should().Be(typeof(PrazoMeses));
        parametros[2].Name.Should().Be("prazo");
    }

    [Fact]
    public void ICalculadoraAmortizacao_DeveTerNamespaceCorreto()
    {
        // Act & Assert
        typeof(ICalculadoraAmortizacao).Namespace.Should().Be("Hackathon.Domain.Interfaces.Services");
    }

    [Fact]
    public void ICalculadoraAmortizacao_DeveTerNomeCorreto()
    {
        // Act & Assert
        typeof(ICalculadoraAmortizacao).Name.Should().Be("ICalculadoraAmortizacao");
    }

    [Fact]
    public void ICalculadoraAmortizacao_DeveSerImplementavel()
    {
        // Arrange
        var tipo = typeof(ICalculadoraAmortizacao);

        // Act & Assert
        tipo.IsAbstract.Should().BeTrue();
        tipo.IsSealed.Should().BeFalse();
    }
}
