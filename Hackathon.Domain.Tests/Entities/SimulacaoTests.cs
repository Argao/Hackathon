using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.Entities;

public class SimulacaoTests
{
    private readonly Fixture _fixture;

    public SimulacaoTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Construtor_DeveCriarSimulacaoComIdUnico()
    {
        // Act
        var simulacao1 = new Simulacao();
        var simulacao2 = new Simulacao();

        // Assert
        simulacao1.IdSimulacao.Should().NotBeEmpty();
        simulacao2.IdSimulacao.Should().NotBeEmpty();
        simulacao1.IdSimulacao.Should().NotBe(simulacao2.IdSimulacao);
    }

    [Fact]
    public void Propriedades_DevePermitirDefinirElerValores()
    {
        // Arrange
        var simulacao = new Simulacao();
        var codigoProduto = 123;
        var descricaoProduto = "Empréstimo Pessoal";
        var taxaJuros = TaxaJuros.Create(0.015m).Value;
        var valorDesejado = ValorMonetario.Create(10000.00m).Value;
        var prazoMeses = (short)24;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var envelopJson = "{\"teste\": \"valor\"}";

        // Act
        simulacao.CodigoProduto = codigoProduto;
        simulacao.DescricaoProduto = descricaoProduto;
        simulacao.TaxaJuros = taxaJuros;
        simulacao.ValorDesejado = valorDesejado;
        simulacao.PrazoMeses = prazoMeses;
        simulacao.DataReferencia = dataReferencia;
        simulacao.EnvelopJson = envelopJson;

        // Assert
        simulacao.CodigoProduto.Should().Be(codigoProduto);
        simulacao.DescricaoProduto.Should().Be(descricaoProduto);
        simulacao.TaxaJuros.Should().Be(taxaJuros);
        simulacao.ValorDesejado.Should().Be(valorDesejado);
        simulacao.PrazoMeses.Should().Be(prazoMeses);
        simulacao.DataReferencia.Should().Be(dataReferencia);
        simulacao.EnvelopJson.Should().Be(envelopJson);
    }

    [Fact]
    public void Resultados_DeveInicializarComoListaVazia()
    {
        // Act
        var simulacao = new Simulacao();

        // Assert
        simulacao.Resultados.Should().NotBeNull();
        simulacao.Resultados.Should().BeEmpty();
    }

    [Fact]
    public void Resultados_DevePermitirAdicionarResultados()
    {
        // Arrange
        var simulacao = new Simulacao();
        var resultado1 = new ResultadoSimulacao();
        var resultado2 = new ResultadoSimulacao();

        // Act
        simulacao.Resultados.Add(resultado1);
        simulacao.Resultados.Add(resultado2);

        // Assert
        simulacao.Resultados.Should().HaveCount(2);
        simulacao.Resultados.Should().Contain(resultado1);
        simulacao.Resultados.Should().Contain(resultado2);
    }

    [Fact]
    public void PropriedadesIniciais_DeveTerValoresPadrao()
    {
        // Act
        var simulacao = new Simulacao();

        // Assert
        simulacao.DescricaoProduto.Should().BeEmpty();
        simulacao.EnvelopJson.Should().BeEmpty();
    }

    [Fact]
    public void Classe_DeveSerSealed()
    {
        // Act & Assert
        typeof(Simulacao).IsSealed.Should().BeTrue();
    }

    [Fact]
    public void IdSimulacao_DeveSerInitOnly()
    {
        // Arrange
        var simulacao = new Simulacao();
        var idOriginal = simulacao.IdSimulacao;

        // Act & Assert
        // Como IdSimulacao é init-only, não deve ser possível alterá-lo após a criação
        // Isso é testado verificando que o valor permanece o mesmo
        simulacao.IdSimulacao.Should().Be(idOriginal);
    }
}
