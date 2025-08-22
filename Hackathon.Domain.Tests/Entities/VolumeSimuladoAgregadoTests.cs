using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.Entities;

public class VolumeSimuladoAgregadoTests
{
    [Fact]
    public void Construtor_DeveCriarVolumeSimuladoComValoresPadrao()
    {
        // Act
        var volumeSimulado = new VolumeSimuladoAgregado();

        // Assert
        volumeSimulado.CodigoProduto.Should().Be(0);
        volumeSimulado.DescricaoProduto.Should().Be(string.Empty);
        volumeSimulado.TaxaMediaJuro.Should().Be(default(TaxaJuros));
        volumeSimulado.ValorMedioPrestacao.Should().Be(default(ValorMonetario));
        volumeSimulado.ValorTotalDesejado.Should().Be(default(ValorMonetario));
        volumeSimulado.ValorTotalCredito.Should().Be(default(ValorMonetario));
    }

    [Fact]
    public void Propriedades_DevePermitirDefinirEObterValores()
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();
        var codigoProduto = 123;
        var descricaoProduto = "Empréstimo Pessoal";
        var taxaMediaJuro = TaxaJuros.Create(0.015m).Value;
        var valorMedioPrestacao = ValorMonetario.Create(500m).Value;
        var valorTotalDesejado = ValorMonetario.Create(10000m).Value;
        var valorTotalCredito = ValorMonetario.Create(12000m).Value;

        // Act
        volumeSimulado.CodigoProduto = codigoProduto;
        volumeSimulado.DescricaoProduto = descricaoProduto;
        volumeSimulado.TaxaMediaJuro = taxaMediaJuro;
        volumeSimulado.ValorMedioPrestacao = valorMedioPrestacao;
        volumeSimulado.ValorTotalDesejado = valorTotalDesejado;
        volumeSimulado.ValorTotalCredito = valorTotalCredito;

        // Assert
        volumeSimulado.CodigoProduto.Should().Be(codigoProduto);
        volumeSimulado.DescricaoProduto.Should().Be(descricaoProduto);
        volumeSimulado.TaxaMediaJuro.Should().Be(taxaMediaJuro);
        volumeSimulado.ValorMedioPrestacao.Should().Be(valorMedioPrestacao);
        volumeSimulado.ValorTotalDesejado.Should().Be(valorTotalDesejado);
        volumeSimulado.ValorTotalCredito.Should().Be(valorTotalCredito);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    [InlineData(0)]
    [InlineData(-1)]
    public void CodigoProduto_DeveAceitarDiferentesValores(int codigoProduto)
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();

        // Act
        volumeSimulado.CodigoProduto = codigoProduto;

        // Assert
        volumeSimulado.CodigoProduto.Should().Be(codigoProduto);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Empréstimo Pessoal")]
    [InlineData("Financiamento Imobiliário")]
    [InlineData("Crédito Consignado")]
    public void DescricaoProduto_DeveAceitarDiferentesValores(string descricaoProduto)
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();

        // Act
        volumeSimulado.DescricaoProduto = descricaoProduto;

        // Assert
        volumeSimulado.DescricaoProduto.Should().Be(descricaoProduto);
    }

    [Theory]
    [InlineData(0.001)]
    [InlineData(0.01)]
    [InlineData(0.05)]
    [InlineData(0.1)]
    [InlineData(0.5)]
    public void TaxaMediaJuro_DeveAceitarDiferentesTaxas(decimal taxa)
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();
        var taxaJuros = TaxaJuros.Create(taxa).Value;

        // Act
        volumeSimulado.TaxaMediaJuro = taxaJuros;

        // Assert
        volumeSimulado.TaxaMediaJuro.Should().Be(taxaJuros);
        volumeSimulado.TaxaMediaJuro.Taxa.Should().Be(taxa);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(5000)]
    [InlineData(10000)]
    [InlineData(50000)]
    public void ValoresMonetarios_DeveAceitarDiferentesValores(decimal valor)
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();
        var valorMonetario = ValorMonetario.Create(valor).Value;

        // Act
        volumeSimulado.ValorMedioPrestacao = valorMonetario;
        volumeSimulado.ValorTotalDesejado = valorMonetario;
        volumeSimulado.ValorTotalCredito = valorMonetario;

        // Assert
        volumeSimulado.ValorMedioPrestacao.Should().Be(valorMonetario);
        volumeSimulado.ValorTotalDesejado.Should().Be(valorMonetario);
        volumeSimulado.ValorTotalCredito.Should().Be(valorMonetario);
        volumeSimulado.ValorMedioPrestacao.Valor.Should().Be(valor);
        volumeSimulado.ValorTotalDesejado.Valor.Should().Be(valor);
        volumeSimulado.ValorTotalCredito.Valor.Should().Be(valor);
    }

    [Fact]
    public void ValoresMonetarios_DevePermitirOperacoesMatematicas()
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();
        var valorPrestacao = ValorMonetario.Create(500m).Value;
        var valorDesejado = ValorMonetario.Create(10000m).Value;
        var valorCredito = ValorMonetario.Create(12000m).Value;

        // Act
        volumeSimulado.ValorMedioPrestacao = valorPrestacao;
        volumeSimulado.ValorTotalDesejado = valorDesejado;
        volumeSimulado.ValorTotalCredito = valorCredito;

        // Assert
        var diferenca = volumeSimulado.ValorTotalCredito - volumeSimulado.ValorTotalDesejado;
        diferenca.Should().Be(ValorMonetario.Create(2000m).Value);

        var soma = volumeSimulado.ValorMedioPrestacao + volumeSimulado.ValorMedioPrestacao;
        soma.Should().Be(ValorMonetario.Create(1000m).Value);

        var multiplicacao = volumeSimulado.ValorMedioPrestacao * 12;
        multiplicacao.Should().Be(ValorMonetario.Create(6000m).Value);
    }

    [Fact]
    public void TaxaJuros_DevePermitirOperacoesMatematicas()
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();
        var taxaJuros = TaxaJuros.Create(0.015m).Value;

        // Act
        volumeSimulado.TaxaMediaJuro = taxaJuros;

        // Assert
        var taxaPercentual = volumeSimulado.TaxaMediaJuro.ParaPercentual();
        taxaPercentual.Should().Be(1.5m);

        var taxaDobrada = volumeSimulado.TaxaMediaJuro * 2;
        taxaDobrada.Taxa.Should().Be(0.03m);
    }

    [Fact]
    public void VolumeSimuladoAgregado_DeveSerClassePublica()
    {
        // Act & Assert
        typeof(VolumeSimuladoAgregado).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void VolumeSimuladoAgregado_DeveTerPropriedadesPublicas()
    {
        // Arrange
        var tipo = typeof(VolumeSimuladoAgregado);

        // Act & Assert
        tipo.GetProperty("CodigoProduto")!.Should().NotBeNull();
        tipo.GetProperty("DescricaoProduto")!.Should().NotBeNull();
        tipo.GetProperty("TaxaMediaJuro")!.Should().NotBeNull();
        tipo.GetProperty("ValorMedioPrestacao")!.Should().NotBeNull();
        tipo.GetProperty("ValorTotalDesejado")!.Should().NotBeNull();
        tipo.GetProperty("ValorTotalCredito")!.Should().NotBeNull();
    }

    [Fact]
    public void VolumeSimuladoAgregado_DeveSerClasseNaoSealed()
    {
        // Act & Assert
        typeof(VolumeSimuladoAgregado).IsSealed.Should().BeFalse();
    }

    [Fact]
    public void Propriedades_DevePermitirDefinirValoresNulos()
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();

        // Act
        volumeSimulado.DescricaoProduto = null!;

        // Assert
        volumeSimulado.DescricaoProduto.Should().BeNull();
    }

    [Fact]
    public void ValoresMonetarios_DevePermitirComparacoes()
    {
        // Arrange
        var volumeSimulado = new VolumeSimuladoAgregado();
        var valorMenor = ValorMonetario.Create(1000m).Value;
        var valorMaior = ValorMonetario.Create(5000m).Value;

        // Act
        volumeSimulado.ValorMedioPrestacao = valorMenor;
        volumeSimulado.ValorTotalCredito = valorMaior;

        // Assert
        (volumeSimulado.ValorTotalCredito > volumeSimulado.ValorMedioPrestacao).Should().BeTrue();
        (volumeSimulado.ValorMedioPrestacao < volumeSimulado.ValorTotalCredito).Should().BeTrue();
        (volumeSimulado.ValorTotalCredito >= volumeSimulado.ValorMedioPrestacao).Should().BeTrue();
        (volumeSimulado.ValorMedioPrestacao <= volumeSimulado.ValorTotalCredito).Should().BeTrue();
    }
}
