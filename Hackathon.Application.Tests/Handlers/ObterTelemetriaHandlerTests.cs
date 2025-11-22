using Hackathon.Application.Exceptions;
using Hackathon.Application.Handlers;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Application.Tests.Handlers;

public class ObterTelemetriaHandlerTests
{
    private readonly Mock<IMetricaRepository> _mockRepository;
    private readonly Mock<ILogger<ObterTelemetriaHandler>> _mockLogger;
    private readonly ObterTelemetriaHandler _handler;

    public ObterTelemetriaHandlerTests()
    {
        _mockRepository = new Mock<IMetricaRepository>();
        _mockLogger = new Mock<ILogger<ObterTelemetriaHandler>>();
        _handler = new ObterTelemetriaHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ComQueryValida_DeveRetornarTelemetriaResult()
    {
        // Arrange
        var query = new ObterTelemetriaQuery(DateOnly.FromDateTime(DateTime.Today));
        var ct = CancellationToken.None;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        var metricasAgregadas = new List<MetricaAgregada>
        {
            new() { NomeApi = "Simulacao", QtdRequisicoes = 100, TempoMedio = 150.5, TempoMinimo = 50L, TempoMaximo = 300L, PercentualSucesso = 0.955 }
        };

        _mockRepository
            .Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ReturnsAsync(metricasAgregadas);

        // Act
        var result = await _handler.Handle(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.DataReferencia.Should().Be(dataReferencia);
        result.ListaEndpoints.Should().HaveCount(1);
        result.ListaEndpoints[0].NomeApi.Should().Be("Simulacao");
        
        _mockRepository.Verify(
            x => x.ObterMetricasPorDataAsync(dataReferencia, ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComRepositorioRetornandoListaVazia_DeveLancarExcecao()
    {
        // Arrange
        var query = new ObterTelemetriaQuery(DateOnly.FromDateTime(DateTime.Today));
        var ct = CancellationToken.None;
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        _mockRepository
            .Setup(x => x.ObterMetricasPorDataAsync(dataReferencia, ct))
            .ReturnsAsync(new List<MetricaAgregada>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundAppException>(
            () => _handler.Handle(query, ct));

        exception.Message.Should().Contain("Nenhum dado de telemetria encontrado");
        
        _mockRepository.Verify(
            x => x.ObterMetricasPorDataAsync(dataReferencia, ct),
            Times.Once);
    }
}
