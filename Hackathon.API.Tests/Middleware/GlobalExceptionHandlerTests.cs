using System.Net;
using System.Text.Json;
using FluentAssertions;
using Hackathon.API.Middleware;
using Hackathon.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.API.Tests.Middleware;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<GlobalExceptionHandler>> _mockLogger;
    private readonly GlobalExceptionHandler _middleware;
    private readonly DefaultHttpContext _httpContext;

    public GlobalExceptionHandlerTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<GlobalExceptionHandler>>();
        _middleware = new GlobalExceptionHandler(_mockNext.Object, _mockLogger.Object);
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_QuandoNaoHaExcecao_DeveChamarNext()
    {
        // Arrange
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
        _httpContext.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task InvokeAsync_QuandoValidationException_DeveRetornarBadRequest()
    {
        // Arrange
        var validationException = new ValidationException("Erro de validação");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(validationException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _httpContext.Response.ContentType.Should().Be("application/json");

        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        response.GetProperty("statusCode").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
        response.GetProperty("message").GetString().Should().Be("Erro de validação");
        response.GetProperty("details").GetProperty("errors").EnumerateArray().First().GetString().Should().Be("Erro de validação");
    }

    [Fact]
    public async Task InvokeAsync_QuandoSimulacaoException_DeveRetornarUnprocessableEntity()
    {
        // Arrange
        var simulacaoException = new SimulacaoException("Erro na simulação");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(simulacaoException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        _httpContext.Response.ContentType.Should().Be("application/json");

        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        response.GetProperty("statusCode").GetInt32().Should().Be((int)HttpStatusCode.UnprocessableEntity);
        response.GetProperty("message").GetString().Should().Be("Erro na simulação");
        response.GetProperty("details").GetProperty("error").GetString().Should().Be("Erro na simulação");
    }

    [Fact]
    public async Task InvokeAsync_QuandoProdutoNotFoundException_DeveRetornarNotFound()
    {
        // Arrange
        var produtoNotFoundException = new ProdutoNotFoundException("PROD001");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(produtoNotFoundException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        _httpContext.Response.ContentType.Should().Be("application/json");

        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        response.GetProperty("statusCode").GetInt32().Should().Be((int)HttpStatusCode.NotFound);
        response.GetProperty("message").GetString().Should().Be("Produto com código 'PROD001' não encontrado.");
        response.GetProperty("details").GetProperty("error").GetString().Should().Be("Produto com código 'PROD001' não encontrado.");
        response.GetProperty("details").GetProperty("codigoProduto").GetString().Should().Be("PROD001");
    }

    [Fact]
    public async Task InvokeAsync_QuandoDomainException_DeveRetornarBadRequest()
    {
        // Arrange
        var domainException = new ValidationException("Erro de domínio");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(domainException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _httpContext.Response.ContentType.Should().Be("application/json");

        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        response.GetProperty("statusCode").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
        response.GetProperty("message").GetString().Should().Be("Erro de validação");
        response.GetProperty("details").GetProperty("errors").EnumerateArray().First().GetString().Should().Be("Erro de domínio");
    }

    [Fact]
    public async Task InvokeAsync_QuandoExcecaoGenerica_DeveRetornarInternalServerError()
    {
        // Arrange
        var genericException = new InvalidOperationException("Erro genérico");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(genericException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        _httpContext.Response.ContentType.Should().Be("application/json");

        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        response.GetProperty("statusCode").GetInt32().Should().Be((int)HttpStatusCode.InternalServerError);
        response.GetProperty("message").GetString().Should().Be("Erro interno do servidor");
        response.GetProperty("details").GetProperty("error").GetString().Should().Be("Ocorreu um erro inesperado");
    }

    [Fact]
    public async Task InvokeAsync_QuandoExcecao_DeveLogarErro()
    {
        // Arrange
        var exception = new Exception("Erro de teste");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_QuandoExcecao_DeveIncluirTimestampNaResposta()
    {
        // Arrange
        var exception = new Exception("Erro de teste");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        response.GetProperty("timestamp").GetDateTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task InvokeAsync_QuandoValidationExceptionComErros_DeveIncluirErrosNaResposta()
    {
        // Arrange
        var validationException = new ValidationException(new List<string> { "Erro 1", "Erro 2" });
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(validationException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        response.GetProperty("details").GetProperty("errors").EnumerateArray().Should().HaveCount(2);
        response.GetProperty("details").GetProperty("errors").EnumerateArray().First().GetString().Should().Be("Erro 1");
        response.GetProperty("details").GetProperty("errors").EnumerateArray().Last().GetString().Should().Be("Erro 2");
    }

    [Fact]
    public async Task InvokeAsync_QuandoBusinessRuleException_DeveRetornarBadRequest()
    {
        // Arrange
        var businessRuleException = new BusinessRuleException("Regra de negócio violada", "RULE001");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(businessRuleException);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _httpContext.Response.ContentType.Should().Be("application/json");

        var responseBody = await GetResponseBody();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        response.GetProperty("statusCode").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
        response.GetProperty("message").GetString().Should().Be("Regra de negócio violada");
        response.GetProperty("details").GetProperty("error").GetString().Should().Be("Regra de negócio violada");
    }

    private async Task<string> GetResponseBody()
    {
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_httpContext.Response.Body);
        return await reader.ReadToEndAsync();
    }
}
