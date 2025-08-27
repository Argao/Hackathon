using FluentAssertions;
using Hackathon.API.Controllers;
using Hackathon.API.Contracts.Requests;
using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hackathon.API.Tests.Controllers;

public class TelemetriaControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly TelemetriaController _controller;

    public TelemetriaControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new TelemetriaController(_mockMediator.Object);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComDataValidaEComDados_DeveRetornarOkComTelemetriaResponse()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var request = new TelemetriaRequest(dataReferencia);
        
        var telemetriaResult = new TelemetriaResult(
            dataReferencia,
            new List<TelemetriaApiResult>
            {
                new("Simulacao", 100, 150.5, 50L, 300L, 95.5)
            }
        );

        _mockMediator
            .Setup(x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(telemetriaResult);

        // Act
        var result = await _controller.ObterTelemetriaPorDia(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<TelemetriaResponse>();
        
        var response = okResult.Value as TelemetriaResponse;
        response!.DataReferencia.Should().Be(dataReferencia);
        response.ListaEndpoints.Should().HaveCount(1);
        response.ListaEndpoints[0].NomeApi.Should().Be("Simulacao");
        response.ListaEndpoints[0].QtdRequisicoes.Should().Be(100);

        _mockMediator.Verify(
            x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComDataValidaESemDados_DeveRetornarNotFound()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var request = new TelemetriaRequest(dataReferencia);

        _mockMediator
            .Setup(x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new SimulacaoException("Nenhum dado de telemetria encontrado para a data 2024-01-15"));

        // Act
        var result = await _controller.ObterTelemetriaPorDia(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().NotBeNull();

        _mockMediator.Verify(
            x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComDataFutura_DeveLancarArgumentException()
    {
        // Arrange
        var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var request = new TelemetriaRequest(dataFutura);

        _mockMediator
            .Setup(x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Data de referência não pode ser futura"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _controller.ObterTelemetriaPorDia(request));

        exception.Message.Should().Be("Data de referência não pode ser futura");

        _mockMediator.Verify(
            x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComDataPassada_DeveRetornarOkComDados()
    {
        // Arrange
        var dataPassada = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var request = new TelemetriaRequest(dataPassada);
        
        var telemetriaResult = new TelemetriaResult(
            dataPassada,
            new List<TelemetriaApiResult>
            {
                new("Telemetria", 50, 100.0, 30L, 200L, 98.0)
            }
        );

        _mockMediator
            .Setup(x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(telemetriaResult);

        // Act
        var result = await _controller.ObterTelemetriaPorDia(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<TelemetriaResponse>();
        
        var response = okResult.Value as TelemetriaResponse;
        response!.DataReferencia.Should().Be(dataPassada);
        response.ListaEndpoints.Should().HaveCount(1);

        _mockMediator.Verify(
            x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComCancellationToken_DevePassarTokenParaServico()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var request = new TelemetriaRequest(dataReferencia);
        var cancellationToken = new CancellationToken();
        
        var telemetriaResult = new TelemetriaResult(
            dataReferencia,
            new List<TelemetriaApiResult>
            {
                new("Test", 1, 50.0, 50L, 50L, 100.0)
            }
        );

        _mockMediator
            .Setup(x => x.Send(It.IsAny<ObterTelemetriaQuery>(), cancellationToken))
            .ReturnsAsync(telemetriaResult);

        // Act
        await _controller.ObterTelemetriaPorDia(request, cancellationToken);

        // Assert
        _mockMediator.Verify(
            x => x.Send(It.IsAny<ObterTelemetriaQuery>(), cancellationToken),
            Times.Once);
    }

    [Fact]
    public void HealthCheck_DeveRetornarOkComDadosDeSaude()
    {
        // Act
        var result = _controller.HealthCheck();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        
        // Verificar se o objeto retornado tem as propriedades esperadas usando JsonElement
        var jsonString = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonString);
        
        jsonElement.GetProperty("service").GetString().Should().Be("Telemetria");
        jsonElement.GetProperty("status").GetString().Should().Be("healthy");
        jsonElement.GetProperty("timestamp").GetDateTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        jsonElement.GetProperty("version").GetString().Should().Be("1.0.0");
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComMultiplosEndpoints_DeveRetornarTodosOsEndpoints()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var request = new TelemetriaRequest(dataReferencia);
        
        var telemetriaResult = new TelemetriaResult(
            dataReferencia,
            new List<TelemetriaApiResult>
            {
                new("Simulacao", 100, 150.5, 50L, 300L, 95.5),
                new("Telemetria", 25, 75.0, 30L, 150L, 100.0)
            }
        );

        _mockMediator
            .Setup(x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(telemetriaResult);

        // Act
        var result = await _controller.ObterTelemetriaPorDia(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<TelemetriaResponse>();
        
        var response = okResult.Value as TelemetriaResponse;
        response!.ListaEndpoints.Should().HaveCount(2);
        response.ListaEndpoints.Should().Contain(e => e.NomeApi == "Simulacao");
        response.ListaEndpoints.Should().Contain(e => e.NomeApi == "Telemetria");

        _mockMediator.Verify(
            x => x.Send(It.IsAny<ObterTelemetriaQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
