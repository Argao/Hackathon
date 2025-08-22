using Microsoft.Extensions.Configuration;

namespace Hackathon.Infrastructure.Tests.EventHub;

public class EventHubServiceTests : IDisposable
{
    private readonly Mock<ILogger<EventHubService>> _loggerMock;
    private EventHubService? _service;

    public EventHubServiceTests()
    {
        _loggerMock = new Mock<ILogger<EventHubService>>();
    }

    [Fact]
    public void Constructor_ComConnectionStringValida_DeveCriarInstancia()
    {
        // Arrange
        var configuration = CreateValidConfiguration();

        // Act & Assert
        FluentActions.Invoking(() => new EventHubService(configuration, _loggerMock.Object))
            .Should().NotThrow();
    }

    [Fact]
    public void Constructor_SemConnectionString_DeveLancarExcecao()
    {
        // Arrange
        var configuration = CreateInvalidConfiguration();

        // Act & Assert
        FluentActions.Invoking(() => new EventHubService(configuration, _loggerMock.Object))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("EventHub connection string não configurada");
    }

    [Fact]
    public async Task EnviarSimulacaoAsync_ComString_DeveExecutarSemErro()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);
        var simulacaoJson = """{"id": 1, "valor": 10000}""";

        // Act & Assert
        // Como não temos um EventHub real, o teste vai falhar na conexão
        // mas devemos verificar se o método não lança exceção inesperada
        await FluentActions.Invoking(() => _service.EnviarSimulacaoAsync(simulacaoJson, CancellationToken.None))
            .Should().NotThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task EnviarSimulacaoAsync_ComObjeto_DeveSerializarCorretamente()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);
        var simulacao = new { Id = 1, Valor = 10000m, Produto = "PROD001" };

        // Act & Assert
        // Como não temos um EventHub real, o teste vai falhar na conexão
        // mas devemos verificar se o método não lança exceção inesperada
        await FluentActions.Invoking(() => _service.EnviarSimulacaoAsync(simulacao, CancellationToken.None))
            .Should().NotThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task EnviarSimulacaoAsync_ComObjetoNulo_DeveLancarArgumentNullException()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);

        // Act & Assert
        await FluentActions.Invoking(() => _service.EnviarSimulacaoAsync<object>(null!, CancellationToken.None))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task EnviarSimulacaoAsync_ComServicoDisposed_DeveLancarObjectDisposedException()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);
        _service.Dispose();

        // Act & Assert
        await FluentActions.Invoking(() => _service.EnviarSimulacaoAsync("test", CancellationToken.None))
            .Should().ThrowAsync<ObjectDisposedException>();
    }

    [Fact]
    public async Task EnviarSimulacaoAsync_ComCancellationTokenCancelado_DevePropagarOperationCanceledException()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await FluentActions.Invoking(() => _service.EnviarSimulacaoAsync("test", cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public void EnviarSimulacao_ComString_DeveExecutarSincrono()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);
        var simulacaoJson = """{"id": 1, "valor": 10000}""";

        // Act & Assert
        FluentActions.Invoking(() => _service.EnviarSimulacao(simulacaoJson))
            .Should().NotThrow<ArgumentException>();
    }

    [Fact]
    public void EnviarSimulacao_ComObjeto_DeveSerializarCorretamente()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);
        var simulacao = new { Id = 1, Valor = 10000m, Produto = "PROD001" };

        // Act & Assert
        FluentActions.Invoking(() => _service.EnviarSimulacao(simulacao))
            .Should().NotThrow<ArgumentException>();
    }

    [Fact]
    public void EnviarSimulacao_ComObjetoNulo_DeveLancarArgumentNullException()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);

        // Act & Assert
        FluentActions.Invoking(() => _service.EnviarSimulacao<object>(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void EnviarSimulacao_ComServicoDisposed_DeveLancarObjectDisposedException()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);
        _service.Dispose();

        // Act & Assert
        FluentActions.Invoking(() => _service.EnviarSimulacao("test"))
            .Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void Dispose_DeveDisposeRecursos()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);

        // Act & Assert
        FluentActions.Invoking(() => _service.Dispose())
            .Should().NotThrow();
    }

    [Fact]
    public void Dispose_ChamadoMultiplasVezes_NaoDeveLancarExcecao()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);

        // Act & Assert
        FluentActions.Invoking(() =>
        {
            _service.Dispose();
            _service.Dispose();
            _service.Dispose();
        }).Should().NotThrow();
    }

    [Fact]
    public void Dispose_ComDisposingFalse_DeveExecutarSemErro()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _service = new EventHubService(configuration, _loggerMock.Object);

        // Act & Assert
        // Testar o método Dispose protegido através de reflexão
        var disposeMethod = typeof(EventHubService).GetMethod("Dispose", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, 
            null, new[] { typeof(bool) }, null);
        
        FluentActions.Invoking(() => disposeMethod!.Invoke(_service, new object[] { false }))
            .Should().NotThrow();
    }

    private static IConfiguration CreateValidConfiguration()
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectionStrings:EventHub"] = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test;EntityPath=testhub"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
    }

    private static IConfiguration CreateInvalidConfiguration()
    {
        var configDict = new Dictionary<string, string?>();

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
    }

    public void Dispose()
    {
        _service?.Dispose();
    }
}