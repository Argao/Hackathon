using FluentAssertions;
using Hackathon.API.Contracts.Responses;

namespace Hackathon.API.Tests.Contracts.Responses;

public class SimulacaoResponseTests
{
    [Fact]
    public void SimulacaoResponse_DeveTerPropriedadesCorretas()
    {
        // Arrange
        var simulacaoId = Guid.NewGuid();
        var resultadoSimulacao = new List<ResultadoSimulacaoResponse>
        {
            new("SAC", new List<ParcelaResponse>())
        };

        // Act
        var response = new SimulacaoResponse(
            simulacaoId,
            1,
            "Produto Teste",
            0.05m,
            resultadoSimulacao
        );

        // Assert
        response.IdSimulacao.Should().Be(simulacaoId);
        response.CodigoProduto.Should().Be(1);
        response.DescricaoProduto.Should().Be("Produto Teste");
        response.TaxaJuros.Should().Be(0.05m);
        response.ResultadoSimulacao.Should().BeEquivalentTo(resultadoSimulacao);
    }

    [Fact]
    public void SimulacaoResponse_ComListaVazia_DeveFuncionar()
    {
        // Arrange
        var simulacaoId = Guid.NewGuid();

        // Act
        var response = new SimulacaoResponse(
            simulacaoId,
            1,
            "Produto Teste",
            0.05m,
            new List<ResultadoSimulacaoResponse>()
        );

        // Assert
        response.IdSimulacao.Should().Be(simulacaoId);
        response.ResultadoSimulacao.Should().NotBeNull();
        response.ResultadoSimulacao.Should().BeEmpty();
    }

    [Fact]
    public void SimulacaoResponse_ComMultiplosResultados_DeveFuncionar()
    {
        // Arrange
        var simulacaoId = Guid.NewGuid();
        var resultadoSimulacao = new List<ResultadoSimulacaoResponse>
        {
            new("SAC", new List<ParcelaResponse>()),
            new("PRICE", new List<ParcelaResponse>())
        };

        // Act
        var response = new SimulacaoResponse(
            simulacaoId,
            1,
            "Produto Teste",
            0.05m,
            resultadoSimulacao
        );

        // Assert
        response.IdSimulacao.Should().Be(simulacaoId);
        response.ResultadoSimulacao.Should().HaveCount(2);
        response.ResultadoSimulacao.Should().Contain(r => r.Tipo == "SAC");
        response.ResultadoSimulacao.Should().Contain(r => r.Tipo == "PRICE");
    }

    [Fact]
    public void ResultadoSimulacaoResponse_DeveTerPropriedadesCorretas()
    {
        // Arrange & Act
        var response = new ResultadoSimulacaoResponse(
            "SAC",
            new List<ParcelaResponse>()
        );

        // Assert
        response.Tipo.Should().Be("SAC");
        response.Parcelas.Should().NotBeNull();
        response.Parcelas.Should().BeEmpty();
    }

    [Fact]
    public void ParcelaResponse_DeveTerPropriedadesCorretas()
    {
        // Arrange & Act
        var response = new ParcelaResponse(
            1,
            800m,
            200m,
            1000m
        );

        // Assert
        response.Numero.Should().Be(1);
        response.ValorAmortizacao.Should().Be(800m);
        response.ValorJuros.Should().Be(200m);
        response.ValorPrestacao.Should().Be(1000m);
    }
}
