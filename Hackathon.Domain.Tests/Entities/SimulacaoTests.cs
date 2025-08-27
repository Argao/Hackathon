using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;
using Hackathon.Domain.Exceptions;
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
    public void Create_ComDadosValidos_DeveCriarSimulacaoComIdUnico()
    {
        // Arrange
        var codigoProduto = 123;
        var descricaoProduto = "Empréstimo Pessoal";
        var taxaJuros = TaxaJuros.Create(0.015m).Value;
        var valorDesejado = ValorMonetario.Create(10000.00m).Value;
        var prazoMeses = PrazoMeses.Create(24).Value;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var simulacao1 = Simulacao.Create(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, prazoMeses, dataReferencia);
        var simulacao2 = Simulacao.Create(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, prazoMeses, dataReferencia);

        // Assert
        simulacao1.IdSimulacao.Should().NotBeEmpty();
        simulacao2.IdSimulacao.Should().NotBeEmpty();
        simulacao1.IdSimulacao.Should().NotBe(simulacao2.IdSimulacao);
    }

    [Fact]
    public void Create_ComDadosValidos_DeveCriarSimulacaoComPropriedadesCorretas()
    {
        // Arrange
        var codigoProduto = 123;
        var descricaoProduto = "Empréstimo Pessoal";
        var taxaJuros = TaxaJuros.Create(0.015m).Value;
        var valorDesejado = ValorMonetario.Create(10000.00m).Value;
        var prazoMeses = PrazoMeses.Create(24).Value;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var simulacao = Simulacao.Create(codigoProduto, descricaoProduto, taxaJuros, valorDesejado, prazoMeses, dataReferencia);

        // Assert
        simulacao.CodigoProduto.Should().Be(codigoProduto);
        simulacao.DescricaoProduto.Should().Be(descricaoProduto);
        simulacao.TaxaJuros.Should().Be(taxaJuros);
        simulacao.ValorDesejado.Should().Be(valorDesejado);
        simulacao.PrazoMeses.Should().Be(prazoMeses);
        simulacao.DataReferencia.Should().Be(dataReferencia);
    }

    [Fact]
    public void Create_ComDescricaoInvalida_DeveLancarExcecao()
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(0.015m).Value;
        var valorDesejado = ValorMonetario.Create(10000.00m).Value;
        var prazoMeses = PrazoMeses.Create(24).Value;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act & Assert
        var action = () => Simulacao.Create(123, "", taxaJuros, valorDesejado, prazoMeses, dataReferencia);
        action.Should().Throw<BusinessRuleException>().WithMessage("*obrigatória*");
    }

    [Fact]
    public void Create_ComDataFutura_DeveLancarExcecao()
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(0.015m).Value;
        var valorDesejado = ValorMonetario.Create(10000.00m).Value;
        var prazoMeses = PrazoMeses.Create(24).Value;
        var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        // Act & Assert
        var action = () => Simulacao.Create(123, "Produto", taxaJuros, valorDesejado, prazoMeses, dataFutura);
        action.Should().Throw<BusinessRuleException>().WithMessage("*futura*");
    }

    [Fact]
    public void Resultados_DeveInicializarComoListaVazia()
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(0.015m).Value;
        var valorDesejado = ValorMonetario.Create(10000.00m).Value;
        var prazoMeses = PrazoMeses.Create(24).Value;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var simulacao = Simulacao.Create(123, "Produto", taxaJuros, valorDesejado, prazoMeses, dataReferencia);

        // Assert
        simulacao.Resultados.Should().NotBeNull();
        simulacao.Resultados.Should().BeEmpty();
    }

    [Fact]
    public void AdicionarResultado_ComResultadoValido_DeveAdicionar()
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(0.015m).Value;
        var valorDesejado = ValorMonetario.Create(10000.00m).Value;
        var prazoMeses = PrazoMeses.Create(24).Value;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var simulacao = Simulacao.Create(123, "Produto", taxaJuros, valorDesejado, prazoMeses, dataReferencia);
        var resultado = new ResultadoSimulacao();

        // Act
        simulacao.AdicionarResultado(resultado);

        // Assert
        simulacao.Resultados.Should().HaveCount(1);
        simulacao.Resultados.Should().Contain(resultado);
    }

    [Fact]
    public void AdicionarResultado_ComResultadoNulo_DeveLancarExcecao()
    {
        // Arrange
        var taxaJuros = TaxaJuros.Create(0.015m).Value;
        var valorDesejado = ValorMonetario.Create(10000.00m).Value;
        var prazoMeses = PrazoMeses.Create(24).Value;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var simulacao = Simulacao.Create(123, "Produto", taxaJuros, valorDesejado, prazoMeses, dataReferencia);

        // Act & Assert
        var action = () => simulacao.AdicionarResultado(null!);
        action.Should().Throw<BusinessRuleException>().WithMessage("*nulo*");
    }

    [Fact]
    public void Classe_DeveSerSealed()
    {
        typeof(Simulacao).IsSealed.Should().BeTrue();
    }
}
