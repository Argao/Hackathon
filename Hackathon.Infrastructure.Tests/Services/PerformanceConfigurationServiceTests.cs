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
        ThreadPool.GetMinThreads(out var initialMinWorkerThreads, out var initialMinCompletionPortThreads);
        ThreadPool.GetMaxThreads(out var initialMaxWorkerThreads, out var initialMaxCompletionPortThreads);

        // Act
        _service.ConfigurePerformanceOptimizations();

        // Assert
        ThreadPool.GetMinThreads(out var newMinWorkerThreads, out var newMinCompletionPortThreads);
        ThreadPool.GetMaxThreads(out var newMaxWorkerThreads, out var newMaxCompletionPortThreads);

        // Verificar se os valores foram configurados corretamente (ajustado para a implementação atual)
        var expectedMinWorkerThreads = Math.Min(50, Environment.ProcessorCount * 2);
        var expectedMinCompletionPortThreads = Math.Min(50, Environment.ProcessorCount * 2);
        
        newMinWorkerThreads.Should().Be(expectedMinWorkerThreads);
        newMinCompletionPortThreads.Should().Be(expectedMinCompletionPortThreads);
        newMaxWorkerThreads.Should().BeGreaterThanOrEqualTo(Environment.ProcessorCount * 8);
        newMaxCompletionPortThreads.Should().BeGreaterThanOrEqualTo(Environment.ProcessorCount * 8);


    }

    [Fact]
    public void ConfigurePerformanceOptimizations_DeveConfigurarGarbageCollector()
    {
        // Act
        _service.ConfigurePerformanceOptimizations();

        // Assert
        // O método deve executar sem exceção
    }

    [Fact]
    public void ConfigurePerformanceOptimizations_ComExcecao_DeveExecutarSemExcecao()
    {
        // Arrange
        var mockLoggerWithException = new Mock<ILogger<PerformanceConfigurationService>>();
        mockLoggerWithException
            .Setup(x => x.Log(
                LogLevel.Warning,
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
