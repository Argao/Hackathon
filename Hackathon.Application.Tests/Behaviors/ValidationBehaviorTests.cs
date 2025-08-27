using Hackathon.Application.Behaviors;
using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Domain.Exceptions;
using MediatR;
using Moq;

namespace Hackathon.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidationService> _mockValidationService;
    private readonly ValidationBehavior<RealizarSimulacaoCommand, object> _behavior;

    public ValidationBehaviorTests()
    {
        _mockValidationService = new Mock<IValidationService>();
        _behavior = new ValidationBehavior<RealizarSimulacaoCommand, object>(_mockValidationService.Object);
    }

    [Fact]
    public async Task Handle_ComValidacaoValida_DeveContinuarPipeline()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;
        var expectedResponse = new { Success = true };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockValidationService
            .Setup(x => x.ValidateAsync(request, ct))
            .ReturnsAsync(validationResult);

        RequestHandlerDelegate<object> next = (ct) => Task.FromResult<object>(expectedResponse);

        // Act
        var result = await _behavior.Handle(request, next, ct);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _mockValidationService.Verify(x => x.ValidateAsync(request, ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComValidacaoInvalida_DeveLancarValidationException()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Valor", "Valor deve ser maior que zero"));

        _mockValidationService
            .Setup(x => x.ValidateAsync(request, ct))
            .ReturnsAsync(validationResult);

        RequestHandlerDelegate<object> next = (ct) => Task.FromResult<object>(new { Success = true });

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Valor deve ser maior que zero*");
    }

    [Fact]
    public async Task Handle_ComValidacaoServiceLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        _mockValidationService
            .Setup(x => x.ValidateAsync(request, ct))
            .ThrowsAsync(new Exception("Erro no serviço de validação"));

        RequestHandlerDelegate<object> next = (ct) => Task.FromResult<object>(new { Success = true });

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Erro no serviço de validação");
    }

    [Fact]
    public async Task Handle_ComNextLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var request = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockValidationService
            .Setup(x => x.ValidateAsync(request, ct))
            .ReturnsAsync(validationResult);

        RequestHandlerDelegate<object> next = (ct) => throw new InvalidOperationException("Erro no handler");

        // Act & Assert
        var action = () => _behavior.Handle(request, next, ct);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro no handler");
    }
}
