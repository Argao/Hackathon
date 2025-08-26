using Hackathon.Application.Services;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Tests.Services;

public class TelemetriaServiceTests
{
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly Mock<IServiceScope> _mockScope;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IMetricaRepository> _mockMetricaRepository;
    private readonly Mock<ILogger<TelemetriaService>> _mockLogger;

    private readonly TelemetriaService _service;

    public TelemetriaServiceTests()
    {
        _mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockScope = new Mock<IServiceScope>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockMetricaRepository = new Mock<IMetricaRepository>();
        _mockLogger = new Mock<ILogger<TelemetriaService>>();

        _mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
        _mockScopeFactory.Setup(x => x.CreateScope()).Returns(_mockScope.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(IMetricaRepository)))
            .Returns(_mockMetricaRepository.Object);

        _service = new TelemetriaService(_mockScopeFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task RegistrarMetricaAsync_ComParametrosValidos_DeveRegistrarMetrica()
    {
        // Arrange
        var nomeApi = "teste-api";
        var endpoint = "/api/teste";
        var tempoResposta = 150L;
        var sucesso = true;
        var statusCode = 200;
        var ct = CancellationToken.None;

        _mockMetricaRepository.Setup(x => x.SalvarMetricaAsync(It.IsAny<MetricaRequisicao>(), ct))
            .Returns(Task.CompletedTask);

        // Act
        await _service.RegistrarMetricaAsync(nomeApi, endpoint, tempoResposta, sucesso, statusCode, ct);

        // Assert
        // Como é fire-and-forget, verificamos apenas se o método não lança exceção
        // A verificação real seria feita através de logs ou verificando o comportamento assíncrono
        // Aguardamos um pouco para permitir que a tarefa assíncrona seja executada
        await Task.Delay(100);
        
        // Verificamos se o logger foi chamado (indicando que a métrica foi enfileirada)
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Métrica enfileirada")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("", "/teste", 100L, true, 200)]
    [InlineData(null, "/teste", 100L, true, 200)]
    [InlineData("API Teste", "", 100L, true, 200)]
    [InlineData("API Teste", null, 100L, true, 200)]
    [InlineData("   ", "/teste", 100L, true, 200)]
    [InlineData("API Teste", "   ", 100L, true, 200)]
    public async Task RegistrarMetricaAsync_ComParametrosInvalidos_DeveLogarWarningENaoRegistrar(string nomeApi, string endpoint, long tempoResposta, bool sucesso, int statusCode)
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        await _service.RegistrarMetricaAsync(nomeApi, endpoint, tempoResposta, sucesso, statusCode, ct);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("parâmetros inválidos")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        
        // Não deve criar scope para parâmetros inválidos
        _mockScopeFactory.Verify(x => x.CreateScope(), Times.Never);
    }

    [Fact]
    public async Task RegistrarMetricaAsync_ComErroNoRepository_DeveLogarErroENaoPropagarExcecao()
    {
        // Arrange
        var nomeApi = "API Teste";
        var endpoint = "/teste";
        var tempoResposta = 100L;
        var sucesso = true;
        var statusCode = 200;
        var ct = CancellationToken.None;

        _mockMetricaRepository.Setup(x => x.SalvarMetricaAsync(It.IsAny<MetricaRequisicao>(), ct))
            .ThrowsAsync(new Exception("Erro no repository"));

        // Act
        await _service.RegistrarMetricaAsync(nomeApi, endpoint, tempoResposta, sucesso, statusCode, ct);

        // Assert
        // O método não deve lançar exceção mesmo com erro no repository
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Erro crítico no serviço de telemetria")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }


}
