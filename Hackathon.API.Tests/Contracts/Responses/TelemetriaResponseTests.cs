using FluentAssertions;
using Hackathon.API.Contracts.Responses;

namespace Hackathon.API.Tests.Contracts.Responses;

public class TelemetriaResponseTests
{
    [Fact]
    public void TelemetriaResponse_DeveTerPropriedadesCorretas()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var listaEndpoints = new List<TelemetriaEndpointResponse>
        {
            new("Simulacao", 100, 150.5, 50L, 300L, 95.5)
        };

        // Act
        var response = new TelemetriaResponse(dataReferencia, listaEndpoints);

        // Assert
        response.DataReferencia.Should().Be(dataReferencia);
        response.ListaEndpoints.Should().BeEquivalentTo(listaEndpoints);
    }

    [Fact]
    public void TelemetriaResponse_ComListaVazia_DeveFuncionar()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var response = new TelemetriaResponse(dataReferencia, new List<TelemetriaEndpointResponse>());

        // Assert
        response.DataReferencia.Should().Be(dataReferencia);
        response.ListaEndpoints.Should().NotBeNull();
        response.ListaEndpoints.Should().BeEmpty();
    }

    [Fact]
    public void TelemetriaResponse_ComMultiplosEndpoints_DeveFuncionar()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var listaEndpoints = new List<TelemetriaEndpointResponse>
        {
            new("Simulacao", 100, 150.5, 50L, 300L, 95.5),
            new("Telemetria", 25, 75.0, 30L, 150L, 100.0)
        };

        // Act
        var response = new TelemetriaResponse(dataReferencia, listaEndpoints);

        // Assert
        response.DataReferencia.Should().Be(dataReferencia);
        response.ListaEndpoints.Should().HaveCount(2);
        response.ListaEndpoints.Should().Contain(e => e.NomeApi == "Simulacao");
        response.ListaEndpoints.Should().Contain(e => e.NomeApi == "Telemetria");
    }

    [Fact]
    public void TelemetriaEndpointResponse_DeveTerPropriedadesCorretas()
    {
        // Arrange & Act
        var endpoint = new TelemetriaEndpointResponse(
            "Teste",
            50,
            100.0,
            25L,
            200L,
            98.0
        );

        // Assert
        endpoint.NomeApi.Should().Be("Teste");
        endpoint.QtdRequisicoes.Should().Be(50);
        endpoint.TempoMedio.Should().Be(100.0);
        endpoint.TempoMinimo.Should().Be(25L);
        endpoint.TempoMaximo.Should().Be(200L);
        endpoint.PercentualSucesso.Should().Be(98.0);
    }
}
