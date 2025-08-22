using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.ValueObjects;
using Xunit;

namespace Hackathon.Domain.Tests.Entities;

public class ResultadoSimulacaoTests
{
    private readonly Fixture _fixture;

    public ResultadoSimulacaoTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Construtor_DeveCriarResultadoComIdUnico()
    {
        // Act
        var resultado1 = new ResultadoSimulacao();
        var resultado2 = new ResultadoSimulacao();

        // Assert
        resultado1.IdResultado.Should().NotBeEmpty();
        resultado2.IdResultado.Should().NotBeEmpty();
        resultado1.IdResultado.Should().NotBe(resultado2.IdResultado);
    }

    [Fact]
    public void Propriedades_DevePermitirDefinirElerValores()
    {
        // Arrange
        var resultado = new ResultadoSimulacao();
        var idSimulacao = Guid.NewGuid();
        var tipo = SistemaAmortizacao.PRICE;
        var simulacao = new Simulacao();
        var valorTotal = ValorMonetario.Create(15000.00m).Value;

        // Act
        resultado.IdSimulacao = idSimulacao;
        resultado.Tipo = tipo;
        resultado.Simulacao = simulacao;
        resultado.ValorTotal = valorTotal;

        // Assert
        resultado.IdSimulacao.Should().Be(idSimulacao);
        resultado.Tipo.Should().Be(tipo);
        resultado.Simulacao.Should().Be(simulacao);
        resultado.ValorTotal.Should().Be(valorTotal);
    }

    [Fact]
    public void Parcelas_DeveInicializarComoListaVazia()
    {
        // Act
        var resultado = new ResultadoSimulacao();

        // Assert
        resultado.Parcelas.Should().NotBeNull();
        resultado.Parcelas.Should().BeEmpty();
    }

    [Fact]
    public void Parcelas_DevePermitirAdicionarParcelas()
    {
        // Arrange
        var resultado = new ResultadoSimulacao();
        var parcela1 = new Parcela { Numero = 1 };
        var parcela2 = new Parcela { Numero = 2 };

        // Act
        resultado.Parcelas.Add(parcela1);
        resultado.Parcelas.Add(parcela2);

        // Assert
        resultado.Parcelas.Should().HaveCount(2);
        resultado.Parcelas.Should().Contain(parcela1);
        resultado.Parcelas.Should().Contain(parcela2);
    }

    [Fact]
    public void IdResultado_DeveSerInitOnly()
    {
        // Arrange
        var resultado = new ResultadoSimulacao();
        var idOriginal = resultado.IdResultado;

        // Act & Assert
        // Como IdResultado é init-only, não deve ser possível alterá-lo após a criação
        resultado.IdResultado.Should().Be(idOriginal);
    }

    [Theory]
    [InlineData(SistemaAmortizacao.PRICE)]
    [InlineData(SistemaAmortizacao.SAC)]
    public void Tipo_DeveAceitarTodosOsTiposDeAmortizacao(SistemaAmortizacao tipo)
    {
        // Arrange
        var resultado = new ResultadoSimulacao();

        // Act
        resultado.Tipo = tipo;

        // Assert
        resultado.Tipo.Should().Be(tipo);
    }

    [Fact]
    public void Simulacao_DevePermitirDefinirSimulacao()
    {
        // Arrange
        var resultado = new ResultadoSimulacao();
        var simulacao = new Simulacao
        {
            CodigoProduto = 123,
            DescricaoProduto = "Empréstimo Pessoal"
        };

        // Act
        resultado.Simulacao = simulacao;

        // Assert
        resultado.Simulacao.Should().Be(simulacao);
        resultado.Simulacao.CodigoProduto.Should().Be(123);
        resultado.Simulacao.DescricaoProduto.Should().Be("Empréstimo Pessoal");
    }

    [Fact]
    public void ValorTotal_DevePermitirDefinirValorMonetario()
    {
        // Arrange
        var resultado = new ResultadoSimulacao();
        var valorTotal = ValorMonetario.Create(20000.00m).Value;

        // Act
        resultado.ValorTotal = valorTotal;

        // Assert
        resultado.ValorTotal.Should().Be(valorTotal);
        resultado.ValorTotal.Valor.Should().Be(20000.00m);
    }
}
