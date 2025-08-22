using FluentAssertions;
using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class TelemetriaResponseDTOTests
{
    [Fact]
    public void TelemetriaResponseDTO_Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.Today);
        var endpoints = new List<TelemetriaEndpointDTO> 
        { 
            new("API1", "/api/test", 100, 1.5, 100, 200, 0.8)
        };

        // Act
        var response = new TelemetriaResponseDTO(data, endpoints);

        // Assert
        response.DataReferencia.Should().Be(data);
        response.ListaEndpoints.Should().BeEquivalentTo(endpoints);
    }

    [Fact]
    public void TelemetriaEndpointDTO_Constructor_ShouldInitializeCorrectly()
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
        var endpointDTO = new TelemetriaEndpointDTO(nomeApi, endpoint, qtdRequisicoes, tempoMedio, tempoMinimo, tempoMaximo, percentualSucesso);

        // Assert
        endpointDTO.NomeApi.Should().Be(nomeApi);
        endpointDTO.Endpoint.Should().Be(endpoint);
        endpointDTO.QtdRequisicoes.Should().Be(qtdRequisicoes);
        endpointDTO.TempoMedio.Should().Be(tempoMedio);
        endpointDTO.TempoMinimo.Should().Be(tempoMinimo);
        endpointDTO.TempoMaximo.Should().Be(tempoMaximo);
        endpointDTO.PercentualSucesso.Should().Be(percentualSucesso);
    }

    [Fact]
    public void TelemetriaApiDTO_Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        const string nomeApi = "SimulacaoAPI";
        const int qtdRequisicoes = 500;
        const double tempoMedio = 1.8;
        const long tempoMinimo = 1000;
        const long tempoMaximo = 25;
        const double percentualSucesso = 0.025;

        // Act
        var apiDTO = new TelemetriaApiDTO(nomeApi, qtdRequisicoes, tempoMedio, tempoMinimo, tempoMaximo, percentualSucesso);

        // Assert
        apiDTO.NomeApi.Should().Be(nomeApi);
        apiDTO.QtdRequisicoes.Should().Be(qtdRequisicoes);
        apiDTO.TempoMedio.Should().Be(tempoMedio);
        apiDTO.TempoMinimo.Should().Be(tempoMinimo);
        apiDTO.TempoMaximo.Should().Be(tempoMaximo);
        apiDTO.PercentualSucesso.Should().Be(percentualSucesso);
    }

    [Fact]
    public void TelemetriaFinalResponseDTO_Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.Today);
        var apis = new List<TelemetriaApiDTO> 
        { 
            new("API1", 100, 1.5, 200, 10, 0.05),
            new("API2", 200, 2.0, 400, 20, 0.05)
        };

        // Act
        var response = new TelemetriaFinalResponseDTO(data, apis);

        // Assert
        response.DataReferencia.Should().Be(data);
        response.ListaEndpoints.Should().BeEquivalentTo(apis);
    }
}
