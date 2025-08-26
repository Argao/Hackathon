using Hackathon.Application.Handlers;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Application.Tests.Handlers;

public class ObterTelemetriaHandlerTests
{
    private readonly Mock<ITelemetriaOrchestrator> _mockOrchestrator;
    private readonly ObterTelemetriaHandler _handler;

    public ObterTelemetriaHandlerTests()
    {
        _mockOrchestrator = new Mock<ITelemetriaOrchestrator>();
        _handler = new ObterTelemetriaHandler(_mockOrchestrator.Object);
    }

    [Fact]
    public async Task Handle_ComQueryValida_DeveRetornarTelemetriaResult()
    {
        // Arrange
        var query = new ObterTelemetriaQuery(DateOnly.FromDateTime(DateTime.Today));
        var ct = CancellationToken.None;

        var telemetriaResult = new TelemetriaResult(
            DateOnly.FromDateTime(DateTime.Today),
            new List<TelemetriaApiResult>
            {
                new("Simulacao", 100, 150.5, 50L, 300L, 95.5)
            }
        );

        _mockOrchestrator
            .Setup(x => x.ObterTelemetriaAsync(query, ct))
            .ReturnsAsync(telemetriaResult);

        // Act
        var result = await _handler.Handle(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(telemetriaResult);
        
        _mockOrchestrator.Verify(
            x => x.ObterTelemetriaAsync(query, ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComOrchestratorLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var query = new ObterTelemetriaQuery(DateOnly.FromDateTime(DateTime.Today));
        var ct = CancellationToken.None;
        var expectedException = new InvalidOperationException("Erro de teste");

        _mockOrchestrator
            .Setup(x => x.ObterTelemetriaAsync(query, ct))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, ct));

        exception.Should().Be(expectedException);
        
        _mockOrchestrator.Verify(
            x => x.ObterTelemetriaAsync(query, ct),
            Times.Once);
    }
}
