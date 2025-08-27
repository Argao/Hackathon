using Hackathon.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Infrastructure.Tests.Services;

public class PerformanceConfigurationServiceTests
{
    private readonly Mock<ILogger<PerformanceConfigurationService>> _mockLogger;
    private readonly PerformanceConfigurationService _service;

    public PerformanceConfigurationServiceTests()
    {
        _mockLogger = new Mock<ILogger<PerformanceConfigurationService>>();
        _service = new PerformanceConfigurationService(_mockLogger.Object);
    }

    [Fact]
    public void ConfigurePerformanceOptimizations_DeveConfigurarThreadPool()
    {
        // Arrange
        var initialMinWorkerThreads = ThreadPool.GetMinThreads(out var initialMinCompletionPortThreads);
        var initialMaxWorkerThreads = ThreadPool.GetMaxThreads(out var initialMaxCompletionPortThreads);

        // Act
        _service.ConfigurePerformanceOptimizations();

        // Assert
        var newMinWorkerThreads = ThreadPool.GetMinThreads(out var newMinCompletionPortThreads);
        var newMaxWorkerThreads = ThreadPool.GetMaxThreads(out var newMaxCompletionPortThreads);

        // Verificar se os valores foram configurados corretamente
        newMinWorkerThreads.Should().Be(100);
        newMinCompletionPortThreads.Should().Be(100);
        newMaxWorkerThreads.Should().Be(Environment.ProcessorCount * 4);
        newMaxCompletionPortThreads.Should().Be(Environment.ProcessorCount * 4);

        // Verificar se os logs foram chamados
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ThreadPool configurado")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void ConfigurePerformanceOptimizations_DeveConfigurarGarbageCollector()
    {
        // Act
        _service.ConfigurePerformanceOptimizations();

        // Assert
        // Verificar se o GC foi configurado
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Garbage Collector configurado")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void ConfigurePerformanceOptimizations_ComExcecao_DeveLogarWarning()
    {
        // Arrange
        var mockLoggerWithException = new Mock<ILogger<PerformanceConfigurationService>>();
        mockLoggerWithException
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Throws(new Exception("Erro de teste"));

        var serviceWithException = new PerformanceConfigurationService(mockLoggerWithException.Object);

        // Act
        var action = () => serviceWithException.ConfigurePerformanceOptimizations();

        // Assert
        action.Should().NotThrow();
    }
}
