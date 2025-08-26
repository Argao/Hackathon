using Hackathon.Application.DTOs.Responses;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Application.Services;
using Hackathon.Domain.Exceptions;
using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Application.Tests.Services;

public class TelemetriaOrchestratorTests
{
    private readonly Mock<IMetricaRepository> _mockMetricaRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<TelemetriaOrchestrator>> _mockLogger;
    private readonly TelemetriaOrchestrator _orchestrator;

    public TelemetriaOrchestratorTests()
    {
        _mockMetricaRepository = new Mock<IMetricaRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<TelemetriaOrchestrator>>();
        _orchestrator = new TelemetriaOrchestrator(_mockMetricaRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ObterTelemetriaAsync_ComQueryValida_DeveRetornarTelemetriaResult()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterTelemetriaQuery(dataReferencia);
        var ct = CancellationToken.None;

        var metricasAgregadas = new List<Domain.Interfaces.Repositories.MetricaAgregada>
        {
            new Domain.Interfaces.Repositories.MetricaAgregada
            {
                NomeApi = "Simulacao",
                Endpoint = "/simulacao",
                QtdRequisicoes = 100,
                TempoMedio = 150.5,
                TempoMinimo = 50,
                TempoMaximo = 300,
                PercentualSucesso = 95.5
            }
        };

        var expectedResult = new TelemetriaResult(
            dataReferencia,
            new List<TelemetriaApiResult>
            {
                new("Simulacao", 100, 150.5, 50L, 300L, 95.5)
            }
        );

        _mockMetricaRepository
            .Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ReturnsAsync(metricasAgregadas);

        _mockMapper
            .Setup(x => x.Map<Application.DTOs.Responses.TelemetriaFinalResponseDTO, TelemetriaResult>(It.IsAny<Application.DTOs.Responses.TelemetriaFinalResponseDTO>()))
            .Returns(expectedResult);

        // Act
        var result = await _orchestrator.ObterTelemetriaAsync(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
        
        _mockMetricaRepository.Verify(
            x => x.ObterMetricasPorDataAsync(dataReferencia, ct),
            Times.Once);
        
        _mockMapper.Verify(
            x => x.Map<Application.DTOs.Responses.TelemetriaFinalResponseDTO, TelemetriaResult>(It.IsAny<Application.DTOs.Responses.TelemetriaFinalResponseDTO>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTelemetriaAsync_ComDataFutura_DeveLancarExcecao()
    {
        // Arrange
        var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var query = new ObterTelemetriaQuery(dataFutura);
        var ct = CancellationToken.None;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _orchestrator.ObterTelemetriaAsync(query, ct));

        exception.Message.Should().Contain("não pode ser futura");
    }

    [Fact]
    public async Task ObterTelemetriaAsync_ComListaVazia_DeveLancarSimulacaoException()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterTelemetriaQuery(dataReferencia);
        var ct = CancellationToken.None;

        _mockMetricaRepository
            .Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ReturnsAsync(new List<Domain.Interfaces.Repositories.MetricaAgregada>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SimulacaoException>(
            () => _orchestrator.ObterTelemetriaAsync(query, ct));

        exception.Message.Should().Contain("Nenhum dado de telemetria encontrado");
    }
}
