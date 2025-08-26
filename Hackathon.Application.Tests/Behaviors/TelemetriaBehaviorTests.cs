using Hackathon.Application.Behaviors;
using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Application.Tests.Behaviors;

public class TelemetriaBehaviorTests
{
    private readonly Mock<ITelemetriaService> _mockTelemetriaService;
    private readonly Mock<ILogger<TelemetriaBehavior<RealizarSimulacaoCommand, object>>> _mockLogger;
    private readonly TelemetriaBehavior<RealizarSimulacaoCommand, object> _behavior;

    public TelemetriaBehaviorTests()
    {
        _mockTelemetriaService = new Mock<ITelemetriaService>();
        _mockLogger = new Mock<ILogger<TelemetriaBehavior<RealizarSimulacaoCommand, object>>>();
        _behavior = new TelemetriaBehavior<RealizarSimulacaoCommand, object>(_mockTelemetriaService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ComSucesso_DeveRegistrarTelemetria()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var expectedResponse = new { Success = true };

        RequestHandlerDelegate<object> next = (ct) => Task.FromResult<object>(expectedResponse);

        // Act
        var result = await _behavior.Handle(request, next, ct);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        
        _mockTelemetriaService.Verify(
            x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComExcecao_DeveRegistrarTelemetriaComErro()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var exception = new Exception("Erro no handler");

        RequestHandlerDelegate<object> next = (ct) => throw exception;

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<Exception>();

        _mockTelemetriaService.Verify(
            x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                false,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComTelemetriaServiceLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var expectedResponse = new { Success = true };

        _mockTelemetriaService
            .Setup(x => x.RegistrarMetricaAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro na telemetria"));

        RequestHandlerDelegate<object> next = (ct) => Task.FromResult<object>(expectedResponse);

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<Exception>().WithMessage("Erro na telemetria");
    }

    [Fact]
    public async Task Handle_ComCancellationTokenCancelado_DevePropagarCancellationToken()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        RequestHandlerDelegate<object> next = (ct) => throw new OperationCanceledException();

        // Act & Assert
        var action = () => _behavior.Handle(request, next, cts.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Handle_ComExecucaoLenta_DeveRegistrarTempoCorreto()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var expectedResponse = new { Success = true };

        RequestHandlerDelegate<object> next = async (ct) =>
        {
            await Task.Delay(100, ct);
            return expectedResponse;
        };

        // Act
        var result = await _behavior.Handle(request, next, ct);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        
        _mockTelemetriaService.Verify(
            x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<long>(t => t >= 100),
                true,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
