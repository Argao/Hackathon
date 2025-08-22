using FluentAssertions;
using Hackathon.API.Middleware;
using Hackathon.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.API.Tests.Middleware;

public class TelemetriaMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<TelemetriaMiddleware>> _mockLogger;
    private readonly TelemetriaMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ITelemetriaService> _mockTelemetriaService;

    public TelemetriaMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<TelemetriaMiddleware>>();
        _middleware = new TelemetriaMiddleware(_mockNext.Object, _mockLogger.Object);
        
        _httpContext = new DefaultHttpContext();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockTelemetriaService = new Mock<ITelemetriaService>();
        
        _httpContext.RequestServices = _mockServiceProvider.Object;
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_QuandoEndpointNormal_DeveProcessarERegistrarMetrica()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "/simulacao";
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(ITelemetriaService)))
            .Returns(_mockTelemetriaService.Object);
        
        _mockTelemetriaService
            .Setup(x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
        
        // Aguardar um pouco para o fire-and-forget completar
        await Task.Delay(100);
        
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
    public async Task InvokeAsync_QuandoEndpointIgnorado_DeveProcessarSemRegistrarMetrica()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "/health";
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
        
        // Aguardar um pouco para verificar que não há chamadas assíncronas
        await Task.Delay(100);
        
        _mockServiceProvider.Verify(
            x => x.GetService(typeof(ITelemetriaService)),
            Times.Never);
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/healthz")]
    [InlineData("/ready")]
    [InlineData("/live")]
    [InlineData("/swagger")]
    [InlineData("/swagger/index.html")]
    [InlineData("/favicon.ico")]
    [InlineData("/robots.txt")]
    public async Task InvokeAsync_QuandoEndpointsIgnorados_DeveProcessarSemRegistrarMetrica(string path)
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = path;
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
        
        // Aguardar um pouco para verificar que não há chamadas assíncronas
        await Task.Delay(100);
        
        _mockServiceProvider.Verify(
            x => x.GetService(typeof(ITelemetriaService)),
            Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_QuandoExcecaoOcorre_DeveRegistrarMetricaComSucessoFalse()
    {
        // Arrange
        _httpContext.Request.Method = "POST";
        _httpContext.Request.Path = "/simulacao";
        _httpContext.Response.StatusCode = 500;
        
        var exception = new Exception("Erro de teste");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);
        
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(ITelemetriaService)))
            .Returns(_mockTelemetriaService.Object);
        
        _mockTelemetriaService
            .Setup(x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                false, // Deve ser false quando há exceção
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _middleware.InvokeAsync(_httpContext));
        
        // Aguardar um pouco para o fire-and-forget completar
        await Task.Delay(100);
        
        _mockTelemetriaService.Verify(
            x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                false, // Verificar que sucesso é false
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_QuandoSucesso_DeveRegistrarMetricaComSucessoTrue()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "/simulacao";
        _httpContext.Response.StatusCode = 200;
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(ITelemetriaService)))
            .Returns(_mockTelemetriaService.Object);
        
        _mockTelemetriaService
            .Setup(x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                true, // Deve ser true quando não há exceção
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        // Aguardar um pouco para o fire-and-forget completar
        await Task.Delay(100);
        
        _mockTelemetriaService.Verify(
            x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                true, // Verificar que sucesso é true
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_QuandoServicoTelemetriaNaoDisponivel_DeveLogarDebug()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "/simulacao";
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(ITelemetriaService)))
            .Returns((ITelemetriaService?)null);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
        
        // Aguardar um pouco para o fire-and-forget completar
        await Task.Delay(100);
        
        // Verificar que não há exceção lançada
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_QuandoExcecaoNoRegistroMetrica_DeveLogarDebug()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "/simulacao";
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(ITelemetriaService)))
            .Returns(_mockTelemetriaService.Object);
        
        _mockTelemetriaService
            .Setup(x => x.RegistrarMetricaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro no serviço de telemetria"));

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
        
        // Aguardar um pouco para o fire-and-forget completar
        await Task.Delay(100);
        
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_QuandoPathVazio_DeveIgnorarEndpoint()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "";
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
        
        // Aguardar um pouco para verificar que não há chamadas assíncronas
        await Task.Delay(100);
        
        _mockServiceProvider.Verify(
            x => x.GetService(typeof(ITelemetriaService)),
            Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_QuandoPathNull_DeveIgnorarEndpoint()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = null;
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(x => x(_httpContext), Times.Once);
        
        // Aguardar um pouco para verificar que não há chamadas assíncronas
        await Task.Delay(100);
        
        _mockServiceProvider.Verify(
            x => x.GetService(typeof(ITelemetriaService)),
            Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_QuandoControllerDisponivel_DeveExtrairNomeApiDoController()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "/simulacao";
        
        // Simular route data com controller
        var routeData = new Microsoft.AspNetCore.Routing.RouteData();
        routeData.Values["controller"] = "Simulacao";
        _httpContext.Request.RouteValues = routeData.Values;
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(ITelemetriaService)))
            .Returns(_mockTelemetriaService.Object);
        
        _mockTelemetriaService
            .Setup(x => x.RegistrarMetricaAsync(
                "Simulacao", // Deve usar o nome do controller
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        // Aguardar um pouco para o fire-and-forget completar
        await Task.Delay(100);
        
        _mockTelemetriaService.Verify(
            x => x.RegistrarMetricaAsync(
                "Simulacao",
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_QuandoControllerNaoDisponivel_DeveExtrairNomeApiDoPath()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "/api/simulacao";
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(ITelemetriaService)))
            .Returns(_mockTelemetriaService.Object);
        
        _mockTelemetriaService
            .Setup(x => x.RegistrarMetricaAsync(
                "api", // Deve usar o primeiro segmento do path
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        // Aguardar um pouco para o fire-and-forget completar
        await Task.Delay(100);
        
        _mockTelemetriaService.Verify(
            x => x.RegistrarMetricaAsync(
                "api",
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_QuandoPathSemSegmentos_DeveUsarUnknownComoNomeApi()
    {
        // Arrange
        _httpContext.Request.Method = "GET";
        _httpContext.Request.Path = "/";
        _httpContext.Request.RouteValues = new RouteValueDictionary(); // Sem controller
        
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(ITelemetriaService)))
            .Returns(_mockTelemetriaService.Object);
        
        _mockTelemetriaService
            .Setup(x => x.RegistrarMetricaAsync(
                "Unknown", // Deve usar "Unknown" quando não há segmentos
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        // Aguardar um pouco para o fire-and-forget completar
        await Task.Delay(100);
        
        _mockTelemetriaService.Verify(
            x => x.RegistrarMetricaAsync(
                "Unknown",
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
