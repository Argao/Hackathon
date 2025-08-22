using FluentAssertions;
using Hackathon.API.Controllers;
using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Results;
using Hackathon.Application.DTOs.Responses;
using Hackathon.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.API.Tests.Controllers;

public class TelemetriaControllerTests
{
    private readonly Mock<ITelemetriaService> _mockTelemetriaService;
    private readonly Mock<ILogger<TelemetriaController>> _mockLogger;
    private readonly TelemetriaController _controller;

    public TelemetriaControllerTests()
    {
        _mockTelemetriaService = new Mock<ITelemetriaService>();
        _mockLogger = new Mock<ILogger<TelemetriaController>>();
        _controller = new TelemetriaController(_mockTelemetriaService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComDataValidaEComDados_DeveRetornarOkComTelemetriaResponse()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        
        var telemetriaResult = new TelemetriaFinalResponseDTO(
            dataReferencia,
            new List<TelemetriaApiDTO>
            {
                new("Simulacao", 100, 150.5, 50L, 300L, 95.5)
            }
        );

        _mockTelemetriaService
            .Setup(x => x.ObterTelemetriaPorDataAsync(dataReferencia, It.IsAny<CancellationToken>()))
            .ReturnsAsync(telemetriaResult);

        // Act
        var result = await _controller.ObterTelemetriaPorDia(dataReferencia);

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

        _mockTelemetriaService.Verify(
            x => x.ObterTelemetriaPorDataAsync(dataReferencia, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComDataValidaESemDados_DeveRetornarNotFound()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        
        var telemetriaResult = new TelemetriaFinalResponseDTO(
            dataReferencia,
            new List<TelemetriaApiDTO>()
        );

        _mockTelemetriaService
            .Setup(x => x.ObterTelemetriaPorDataAsync(dataReferencia, It.IsAny<CancellationToken>()))
            .ReturnsAsync(telemetriaResult);

        // Act
        var result = await _controller.ObterTelemetriaPorDia(dataReferencia);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().NotBeNull();

        _mockTelemetriaService.Verify(
            x => x.ObterTelemetriaPorDataAsync(dataReferencia, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComDataFutura_DeveLancarValidationException()
    {
        // Arrange
        var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _controller.ObterTelemetriaPorDia(dataFutura));

        exception.Message.Should().Be("Data de referência não pode ser futura");

        _mockTelemetriaService.Verify(
            x => x.ObterTelemetriaPorDataAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComDataPassada_DeveRetornarOkComDados()
    {
        // Arrange
        var dataPassada = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        
        var telemetriaResult = new TelemetriaFinalResponseDTO(
            dataPassada,
            new List<TelemetriaApiDTO>
            {
                new("Telemetria", 50, 100.0, 30L, 200L, 98.0)
            }
        );

        _mockTelemetriaService
            .Setup(x => x.ObterTelemetriaPorDataAsync(dataPassada, It.IsAny<CancellationToken>()))
            .ReturnsAsync(telemetriaResult);

        // Act
        var result = await _controller.ObterTelemetriaPorDia(dataPassada);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<TelemetriaResponse>();
        
        var response = okResult.Value as TelemetriaResponse;
        response!.DataReferencia.Should().Be(dataPassada);
        response.ListaEndpoints.Should().HaveCount(1);

        _mockTelemetriaService.Verify(
            x => x.ObterTelemetriaPorDataAsync(dataPassada, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTelemetriaPorDia_ComCancellationToken_DevePassarTokenParaServico()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var cancellationToken = new CancellationToken();
        
        var telemetriaResult = new TelemetriaFinalResponseDTO(
            dataReferencia,
            new List<TelemetriaApiDTO>
            {
                new("Test", 1, 50.0, 50L, 50L, 100.0)
            }
        );

        _mockTelemetriaService
            .Setup(x => x.ObterTelemetriaPorDataAsync(dataReferencia, cancellationToken))
            .ReturnsAsync(telemetriaResult);

        // Act
        await _controller.ObterTelemetriaPorDia(dataReferencia, cancellationToken);

        // Assert
        _mockTelemetriaService.Verify(
            x => x.ObterTelemetriaPorDataAsync(dataReferencia, cancellationToken),
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
        
        var telemetriaResult = new TelemetriaFinalResponseDTO(
            dataReferencia,
            new List<TelemetriaApiDTO>
            {
                new("Simulacao", 100, 150.5, 50L, 300L, 95.5),
                new("Telemetria", 25, 75.0, 30L, 150L, 100.0)
            }
        );

        _mockTelemetriaService
            .Setup(x => x.ObterTelemetriaPorDataAsync(dataReferencia, It.IsAny<CancellationToken>()))
            .ReturnsAsync(telemetriaResult);

        // Act
        var result = await _controller.ObterTelemetriaPorDia(dataReferencia);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<TelemetriaResponse>();
        
        var response = okResult.Value as TelemetriaResponse;
        response!.ListaEndpoints.Should().HaveCount(2);
        response.ListaEndpoints.Should().Contain(e => e.NomeApi == "Simulacao");
        response.ListaEndpoints.Should().Contain(e => e.NomeApi == "Telemetria");

        _mockTelemetriaService.Verify(
            x => x.ObterTelemetriaPorDataAsync(dataReferencia, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
