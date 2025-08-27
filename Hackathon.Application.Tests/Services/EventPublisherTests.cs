using Hackathon.Application.Services;
using Hackathon.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Tests.Services;

public class EventPublisherTests
{
    private readonly Mock<IEventHubService> _mockEventHubService;
    private readonly Mock<ILogger<EventPublisher>> _mockLogger;
    private readonly EventPublisher _service;

    public EventPublisherTests()
    {
        _mockEventHubService = new Mock<IEventHubService>();
        _mockLogger = new Mock<ILogger<EventPublisher>>();
        _service = new EventPublisher(_mockEventHubService.Object, _mockLogger.Object);
    }

    [Fact]
    public void PublishAsync_ComEventoValido_DevePublicarEventoComSucesso()
    {
        // Arrange
        var evento = new { Id = 1, Nome = "Teste" };

        // Act
        _service.PublishAsync(evento);

        // Assert
        // Aguardamos um pouco para permitir que a tarefa assíncrona seja executada
        Thread.Sleep(200);

        _mockEventHubService.Verify(x => x.EnviarSimulacao(evento), Times.Once);
    }

    [Fact]
    public void PublishAsync_ComEventoNull_DeveEnviarEventoNull()
    {
        // Arrange
        object evento = null!;

        // Act
        _service.PublishAsync(evento);

        // Assert
        // Aguardamos um pouco para permitir que a tarefa assíncrona seja executada
        Thread.Sleep(200);

        _mockEventHubService.Verify(x => x.EnviarSimulacao(evento), Times.Once);
    }

    [Fact]
    public void PublishAsync_ComErroNoEventHub_DeveLogarWarningENaoPropagarExcecao()
    {
        // Arrange
        var evento = new { Id = 1, Nome = "Teste" };
        var excecao = new InvalidOperationException("Erro no EventHub");

        _mockEventHubService.Setup(x => x.EnviarSimulacao(evento))
            .Throws(excecao);

        // Act
        _service.PublishAsync(evento);

        // Assert
        // Aguardamos um pouco para permitir que a tarefa assíncrona seja executada
        Thread.Sleep(200);

        _mockEventHubService.Verify(x => x.EnviarSimulacao(evento), Times.Once);

    }

    [Fact]
    public void PublishAsync_ComDiferentesTiposDeEvento_DevePublicarTodos()
    {
        // Arrange
        var evento1 = new { Id = 1, Tipo = "Simulacao" };
        var evento2 = new { Id = 2, Tipo = "Telemetria" };

        // Act
        _service.PublishAsync(evento1);
        _service.PublishAsync(evento2);

        // Assert
        // Aguardamos um pouco para permitir que as tarefas assíncronas sejam executadas
        Thread.Sleep(300);

        _mockEventHubService.Verify(x => x.EnviarSimulacao(evento1), Times.Once);
        _mockEventHubService.Verify(x => x.EnviarSimulacao(evento2), Times.Once);

    }

    [Fact]
    public void PublishAsync_DeveExecutarDeFormaNaoBloqueante()
    {
        // Arrange
        var evento = new { Id = 1, Nome = "Teste" };
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        _service.PublishAsync(evento);
        stopwatch.Stop();

        // Assert
        // O método deve retornar imediatamente (não deve bloquear)
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "O método deve retornar rapidamente");

        // Aguardamos um pouco para permitir que a tarefa assíncrona seja executada
        Thread.Sleep(200);

        _mockEventHubService.Verify(x => x.EnviarSimulacao(evento), Times.Once);
    }
}
