using Hackathon.Application.Commands;
using Hackathon.Application.Handlers;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Results;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Application.Tests.Handlers;

public class RealizarSimulacaoHandlerTests
{
    private readonly Mock<ISimulacaoOrchestrator> _mockOrchestrator;
    private readonly RealizarSimulacaoHandler _handler;

    public RealizarSimulacaoHandlerTests()
    {
        _mockOrchestrator = new Mock<ISimulacaoOrchestrator>();
        _handler = new RealizarSimulacaoHandler(_mockOrchestrator.Object);
    }

    [Fact]
    public async Task Handle_ComComandoValido_DeveRetornarSimulacaoResult()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var simulacaoResult = new SimulacaoResult(
            Guid.NewGuid(),
            1,
            "Produto Teste",
            0.015m,
            new List<ResultadoCalculoAmortizacao>
            {
                new("PRICE", new List<ParcelaCalculada>()),
                new("SAC", new List<ParcelaCalculada>())
            }
        );

        _mockOrchestrator
            .Setup(x => x.RealizarSimulacaoAsync(command, ct))
            .ReturnsAsync(simulacaoResult);

        // Act
        var result = await _handler.Handle(command, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(simulacaoResult);
        
        _mockOrchestrator.Verify(
            x => x.RealizarSimulacaoAsync(command, ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComOrchestratorLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        _mockOrchestrator
            .Setup(x => x.RealizarSimulacaoAsync(command, ct))
            .ThrowsAsync(new InvalidOperationException("Erro no orquestrador"));

        // Act & Assert
        var action = () => _handler.Handle(command, ct);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro no orquestrador");
    }

    [Fact]
    public async Task Handle_ComCancellationTokenCancelado_DevePropagarCancellationToken()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockOrchestrator
            .Setup(x => x.RealizarSimulacaoAsync(command, cts.Token))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var action = () => _handler.Handle(command, cts.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();

        _mockOrchestrator.Verify(x => x.RealizarSimulacaoAsync(command, cts.Token), Times.Once);
    }
}
