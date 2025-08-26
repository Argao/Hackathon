using Hackathon.Application.Behaviors;
using Hackathon.Application.Commands;
using Hackathon.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Application.Tests.Behaviors;

public class ExceptionHandlingBehaviorTests
{
    private readonly Mock<ILogger<ExceptionHandlingBehavior<RealizarSimulacaoCommand, object>>> _mockLogger;
    private readonly ExceptionHandlingBehavior<RealizarSimulacaoCommand, object> _behavior;

    public ExceptionHandlingBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<ExceptionHandlingBehavior<RealizarSimulacaoCommand, object>>>();
        _behavior = new ExceptionHandlingBehavior<RealizarSimulacaoCommand, object>(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ComSucesso_DeveRetornarResultado()
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
    }

    [Fact]
    public async Task Handle_ComValidationException_DeveLogarWarningERetornarExcecao()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var validationException = new ValidationException(new[] { "Erro de validação" });

        RequestHandlerDelegate<object> next = (ct) => throw validationException;

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<ValidationException>();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro de validação")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComBusinessRuleException_DeveLogarWarningERetornarExcecao()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var businessException = new BusinessRuleException("Regra violada", "RULE001");

        RequestHandlerDelegate<object> next = (ct) => throw businessException;

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<BusinessRuleException>();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Regra de negócio violada")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComSimulacaoException_DeveLogarWarningERetornarExcecao()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var simulacaoException = new SimulacaoException("Erro na simulação");

        RequestHandlerDelegate<object> next = (ct) => throw simulacaoException;

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<SimulacaoException>();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro na simulação")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComExcecaoGenerica_DeveLogarErrorERetornarExcecao()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var genericException = new Exception("Erro genérico");

        RequestHandlerDelegate<object> next = (ct) => throw genericException;

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<Exception>();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                genericException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
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
}
