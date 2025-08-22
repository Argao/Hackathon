using FluentAssertions;
using Hackathon.Application.Results;

namespace Hackathon.Application.Tests.Results;

public class TelemetriaResultTests
{
    [Fact]
    public void TelemetriaResult_Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.Today);
        var apis = new List<TelemetriaApiResult> 
        { 
            new("API1", 100, 1.5, 200, 10, 0.05),
            new("API2", 200, 2.0, 400, 20, 0.05)
        };

        // Act
        var result = new TelemetriaResult(data, apis);

        // Assert
        result.DataReferencia.Should().Be(data);
        result.ListaEndpoints.Should().BeEquivalentTo(apis);
    }

    [Fact]
    public void TelemetriaApiResult_Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        const string nomeApi = "SimulacaoAPI";
        const int qtdRequisicoes = 500;
        const double tempoMedio = 1.8;
        const long tempoMinimo = 1000;
        const long tempoMaximo = 25;
        const double percentualSucesso = 0.025;

        // Act
        var result = new TelemetriaApiResult(nomeApi, qtdRequisicoes, tempoMedio, tempoMinimo, tempoMaximo, percentualSucesso);

        // Assert
        result.NomeApi.Should().Be(nomeApi);
        result.QtdRequisicoes.Should().Be(qtdRequisicoes);
        result.TempoMedio.Should().Be(tempoMedio);
        result.TempoMinimo.Should().Be(tempoMinimo);
        result.TempoMaximo.Should().Be(tempoMaximo);
        result.PercentualSucesso.Should().Be(percentualSucesso);
    }

    [Fact]
    public void TelemetriaEndpointResult_Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        const string nomeApi = "POST";
        const string endpoint = "/api/simulacao";
        const int qtdRequisicoes = 201;
        const double tempoMedio = 2.5;
        const long tempoMinimo = 150;
        const long tempoMaximo = 5;
        const double percentualSucesso = 0.033;

        // Act
        var result = new TelemetriaEndpointResult(nomeApi, endpoint, qtdRequisicoes, tempoMedio, tempoMinimo, tempoMaximo, percentualSucesso);

        // Assert
        result.NomeApi.Should().Be(nomeApi);
        result.Endpoint.Should().Be(endpoint);
        result.QtdRequisicoes.Should().Be(qtdRequisicoes);
        result.TempoMedio.Should().Be(tempoMedio);
        result.TempoMinimo.Should().Be(tempoMinimo);
        result.TempoMaximo.Should().Be(tempoMaximo);
        result.PercentualSucesso.Should().Be(percentualSucesso);
    }
}
