using Hackathon.Application.Services;
using Hackathon.Application.DTOs.Responses;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Exceptions;
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

    [Fact]
    public async Task ObterTelemetriaPorDataAsync_ComDataValida_DeveRetornarTelemetriaFinalResponseDTO()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var ct = CancellationToken.None;

        var metricasAgregadas = new List<MetricaAgregada>
        {
            new MetricaAgregada
            {
                NomeApi = "API Teste",
                Endpoint = "/teste",
                QtdRequisicoes = 2,
                TempoMedio = 150,
                TempoMinimo = 100,
                TempoMaximo = 200,
                PercentualSucesso = 50.0
            }
        };

        _mockMetricaRepository.Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ReturnsAsync(metricasAgregadas);

        // Act
        var result = await _service.ObterTelemetriaPorDataAsync(dataReferencia, ct);

        // Assert
        result.Should().NotBeNull();
        result.DataReferencia.Should().Be(dataReferencia);
        result.ListaEndpoints.Should().HaveCount(1);
        result.ListaEndpoints[0].NomeApi.Should().Be("API Teste");
        result.ListaEndpoints[0].QtdRequisicoes.Should().Be(2);
        result.ListaEndpoints[0].TempoMedio.Should().Be(150);
        result.ListaEndpoints[0].TempoMinimo.Should().Be(100);
        result.ListaEndpoints[0].TempoMaximo.Should().Be(200);
        result.ListaEndpoints[0].PercentualSucesso.Should().Be(50.0);
    }

    [Fact]
    public async Task ObterTelemetriaPorDataAsync_ComErroNoRepository_DeveLancarSimulacaoException()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var ct = CancellationToken.None;

        _mockMetricaRepository.Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ThrowsAsync(new Exception("Erro no repository"));

        // Act & Assert
        var action = () => _service.ObterTelemetriaPorDataAsync(dataReferencia, ct);
        await action.Should().ThrowAsync<SimulacaoException>()
            .WithMessage("*Erro interno*");
    }

    [Fact]
    public async Task ObterTelemetriaPorDataAsync_ComOperacaoCancelada_DeveLancarSimulacaoException()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var ct = CancellationToken.None;

        _mockMetricaRepository.Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var action = () => _service.ObterTelemetriaPorDataAsync(dataReferencia, ct);
        await action.Should().ThrowAsync<SimulacaoException>()
            .WithMessage("Operação cancelada");
    }

    [Fact]
    public async Task ObterTelemetriaPorDataAsync_ComMetricasVazias_DeveRetornarListaVazia()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var ct = CancellationToken.None;

        _mockMetricaRepository.Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ReturnsAsync(new List<MetricaAgregada>());

        // Act
        var result = await _service.ObterTelemetriaPorDataAsync(dataReferencia, ct);

        // Assert
        result.Should().NotBeNull();
        result.DataReferencia.Should().Be(dataReferencia);
        result.ListaEndpoints.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterTelemetriaPorDataAsync_ComMultiplasAPIs_DeveAgruparCorretamente()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var ct = CancellationToken.None;

        var metricasAgregadas = new List<MetricaAgregada>
        {
            new MetricaAgregada { NomeApi = "API A", Endpoint = "/a1", QtdRequisicoes = 1, TempoMedio = 100, TempoMinimo = 100, TempoMaximo = 100, PercentualSucesso = 100.0 },
            new MetricaAgregada { NomeApi = "API A", Endpoint = "/a2", QtdRequisicoes = 1, TempoMedio = 200, TempoMinimo = 200, TempoMaximo = 200, PercentualSucesso = 100.0 },
            new MetricaAgregada { NomeApi = "API B", Endpoint = "/b1", QtdRequisicoes = 1, TempoMedio = 300, TempoMinimo = 300, TempoMaximo = 300, PercentualSucesso = 0.0 },
            new MetricaAgregada { NomeApi = "API B", Endpoint = "/b2", QtdRequisicoes = 1, TempoMedio = 400, TempoMinimo = 400, TempoMaximo = 400, PercentualSucesso = 100.0 }
        };

        _mockMetricaRepository.Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ReturnsAsync(metricasAgregadas);

        // Act
        var result = await _service.ObterTelemetriaPorDataAsync(dataReferencia, ct);

        // Assert
        result.Should().NotBeNull();
        result.ListaEndpoints.Should().HaveCount(2);
        
        var apiA = result.ListaEndpoints.First(x => x.NomeApi == "API A");
        apiA.QtdRequisicoes.Should().Be(2);
        apiA.TempoMedio.Should().Be(150); // (100 + 200) / 2
        apiA.PercentualSucesso.Should().Be(100.0); // 2 sucessos de 2 requisições
        
        var apiB = result.ListaEndpoints.First(x => x.NomeApi == "API B");
        apiB.QtdRequisicoes.Should().Be(2);
        apiB.TempoMedio.Should().Be(350); // (300 + 400) / 2
        apiB.PercentualSucesso.Should().Be(50.0); // 1 sucesso de 2 requisições
    }
}
