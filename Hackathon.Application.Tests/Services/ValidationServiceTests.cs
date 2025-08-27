using FluentValidation;
using FluentValidation.Results;
using Hackathon.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hackathon.Application.Tests.Services;

public class ValidationServiceTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly ValidationService _service;

    public ValidationServiceTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _service = new ValidationService(_mockServiceProvider.Object);
    }

    [Fact]
    public async Task ValidateAsync_ComValidadorDisponivel_DeveExecutarValidacao()
    {
        // Arrange
        var request = new TestRequest { Id = 1, Nome = "Teste" };
        var mockValidator = new Mock<IValidator<TestRequest>>();
        var validationResult = new ValidationResult();

        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator.Object);

        mockValidator.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _service.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(validationResult, result);
        mockValidator.Verify(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_ComValidadorNaoDisponivel_DeveRetornarResultadoVazio()
    {
        // Arrange
        var request = new TestRequest { Id = 1, Nome = "Teste" };

        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((IValidator<TestRequest>)null);

        // Act
        var result = await _service.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_ComValidacaoComErros_DeveRetornarErros()
    {
        // Arrange
        var request = new TestRequest { Id = 1, Nome = "" };
        var mockValidator = new Mock<IValidator<TestRequest>>();
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new("Nome", "Nome é obrigatório")
        });

        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator.Object);

        mockValidator.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _service.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Nome", result.Errors[0].PropertyName);
        Assert.Equal("Nome é obrigatório", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task ValidateAsync_ComValidacaoValida_DeveRetornarSucesso()
    {
        // Arrange
        var request = new TestRequest { Id = 1, Nome = "Nome Válido" };
        var mockValidator = new Mock<IValidator<TestRequest>>();
        var validationResult = new ValidationResult(); // Válido

        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator.Object);

        mockValidator.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _service.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_ComDiferentesTipos_DeveBuscarValidadorCorreto()
    {
        // Arrange
        var request1 = new TestRequest { Id = 1, Nome = "Teste 1" };
        var request2 = new AnotherTestRequest { Codigo = 2, Descricao = "Teste 2" };

        var mockValidator1 = new Mock<IValidator<TestRequest>>();
        var mockValidator2 = new Mock<IValidator<AnotherTestRequest>>();

        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator1.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<AnotherTestRequest>)))
            .Returns(mockValidator2.Object);

        mockValidator1.Setup(x => x.ValidateAsync(request1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        mockValidator2.Setup(x => x.ValidateAsync(request2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result1 = await _service.ValidateAsync(request1, CancellationToken.None);
        var result2 = await _service.ValidateAsync(request2, CancellationToken.None);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.True(result1.IsValid);
        Assert.True(result2.IsValid);

        mockValidator1.Verify(x => x.ValidateAsync(request1, It.IsAny<CancellationToken>()), Times.Once);
        mockValidator2.Verify(x => x.ValidateAsync(request2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_ComCancellationToken_DevePassarTokenParaValidador()
    {
        // Arrange
        var request = new TestRequest { Id = 1, Nome = "Teste" };
        var mockValidator = new Mock<IValidator<TestRequest>>();
        var cancellationToken = new CancellationToken(true); // Cancelado

        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator.Object);

        mockValidator.Setup(x => x.ValidateAsync(request, cancellationToken))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _service.ValidateAsync(request, cancellationToken);

        // Assert
        Assert.NotNull(result);
        mockValidator.Verify(x => x.ValidateAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_ComValidadorQueLancaExcecao_DevePropagarExcecao()
    {
        // Arrange
        var request = new TestRequest { Id = 1, Nome = "Teste" };
        var mockValidator = new Mock<IValidator<TestRequest>>();
        var excecao = new InvalidOperationException("Erro no validador");

        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator.Object);

        mockValidator.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(excecao);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.ValidateAsync(request, CancellationToken.None));

        Assert.Equal("Erro no validador", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_ComRequestNull_DevePassarNullParaValidador()
    {
        // Arrange
        TestRequest request = null!;
        var mockValidator = new Mock<IValidator<TestRequest>>();

        _mockServiceProvider.Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator.Object);

        mockValidator.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _service.ValidateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        mockValidator.Verify(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Classes de teste
    public class TestRequest
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class AnotherTestRequest
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
    }
}
