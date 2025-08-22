using FluentAssertions;
using Hackathon.API.Controllers;
using Hackathon.API.Contracts.Requests;
using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Hackathon.API.Tests.Controllers;

public class SimulacaoControllerTests
{
    private readonly Mock<ISimulacaoService> _mockSimulacaoService;
    private readonly SimulacaoController _controller;

    public SimulacaoControllerTests()
    {
        _mockSimulacaoService = new Mock<ISimulacaoService>();
        _controller = new SimulacaoController(_mockSimulacaoService.Object);
    }

    [Fact]
    public async Task RealizarSimulacao_ComRequestValido_DeveRetornarOkComSimulacaoResponse()
    {
        // Arrange
        var request = new SimulacaoRequest(10000, 12);

        var simulacaoResult = new SimulacaoResult(
            Guid.NewGuid(),
            1,
            "Produto Teste",
            0.05m,
            new List<ResultadoCalculoAmortizacao>
            {
                new("SAC", new List<ParcelaCalculada>()),
                new("PRICE", new List<ParcelaCalculada>())
            }
        );

        _mockSimulacaoService
            .Setup(x => x.RealizarSimulacaoAsync(It.IsAny<RealizarSimulacaoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(simulacaoResult);

        // Act
        var result = await _controller.RealizarSimulacao(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<SimulacaoResponse>();
        
        var response = okResult.Value as SimulacaoResponse;
        response!.IdSimulacao.Should().Be(simulacaoResult.Id);
        response.ResultadoSimulacao.Should().HaveCount(2);

        _mockSimulacaoService.Verify(
            x => x.RealizarSimulacaoAsync(It.IsAny<RealizarSimulacaoCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ListarSimulacoes_ComRequestValido_DeveRetornarOkComListarSimulacoesResponse()
    {
        // Arrange
        var request = new ListarSimulacoesRequest(1, 10);

        var pagedResult = new PagedResult<SimulacaoResumoResult>(
            new List<SimulacaoResumoResult>
            {
                new(Guid.NewGuid(), 10000, 12, 12000),
                new(Guid.NewGuid(), 15000, 24, 18000)
            },
            5,
            1,
            10
        );

        _mockSimulacaoService
            .Setup(x => x.ListarSimulacoesAsync(It.IsAny<ListarSimulacoesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.ListarSimulacoes(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<ListarSimulacoesResponse>();
        
        var response = okResult.Value as ListarSimulacoesResponse;
        response!.Pagina.Should().Be(1);
        response.QtdRegistros.Should().Be(5);
        response.QtdRegistrosPagina.Should().Be(10);
        response.Registros.Should().HaveCount(2);

        _mockSimulacaoService.Verify(
            x => x.ListarSimulacoesAsync(It.IsAny<ListarSimulacoesQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterVolumePorDia_ComDataValida_DeveRetornarOkComVolumeSimuladoResponse()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        
        var volumeResult = new VolumeSimuladoResult(
            dataReferencia,
            new List<VolumeSimuladoProdutoResult>
            {
                new(1, "Produto 1", 0.05m, 1000m, 10000m, 12000m)
            }
        );

        _mockSimulacaoService
            .Setup(x => x.ObterVolumeSimuladoAsync(It.IsAny<ObterVolumeSimuladoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(volumeResult);

        // Act
        var result = await _controller.ObterVolumePorDia(dataReferencia, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<VolumeSimuladoResponse>();
        
        var response = okResult.Value as VolumeSimuladoResponse;
        response!.DataReferencia.Should().Be(dataReferencia.ToString("yyyy-MM-dd"));
        response.Simulacoes.Should().HaveCount(1);

        _mockSimulacaoService.Verify(
            x => x.ObterVolumeSimuladoAsync(It.IsAny<ObterVolumeSimuladoQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RealizarSimulacao_ComCancellationToken_DevePassarTokenParaServico()
    {
        // Arrange
        var request = new SimulacaoRequest(10000, 12);

        var simulacaoResult = new SimulacaoResult(
            Guid.NewGuid(),
            1,
            "Produto Teste",
            0.05m,
            new List<ResultadoCalculoAmortizacao>()
        );

        var cancellationToken = new CancellationToken();

        _mockSimulacaoService
            .Setup(x => x.RealizarSimulacaoAsync(It.IsAny<RealizarSimulacaoCommand>(), cancellationToken))
            .ReturnsAsync(simulacaoResult);

        // Act
        await _controller.RealizarSimulacao(request, cancellationToken);

        // Assert
        _mockSimulacaoService.Verify(
            x => x.RealizarSimulacaoAsync(It.IsAny<RealizarSimulacaoCommand>(), cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ListarSimulacoes_ComCancellationToken_DevePassarTokenParaServico()
    {
        // Arrange
        var request = new ListarSimulacoesRequest(1, 10);

        var pagedResult = new PagedResult<SimulacaoResumoResult>(
            new List<SimulacaoResumoResult>(),
            0,
            1,
            10
        );

        var cancellationToken = new CancellationToken();

        _mockSimulacaoService
            .Setup(x => x.ListarSimulacoesAsync(It.IsAny<ListarSimulacoesQuery>(), cancellationToken))
            .ReturnsAsync(pagedResult);

        // Act
        await _controller.ListarSimulacoes(request, cancellationToken);

        // Assert
        _mockSimulacaoService.Verify(
            x => x.ListarSimulacoesAsync(It.IsAny<ListarSimulacoesQuery>(), cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ObterVolumePorDia_ComCancellationToken_DevePassarTokenParaServico()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        
        var volumeResult = new VolumeSimuladoResult(
            dataReferencia,
            new List<VolumeSimuladoProdutoResult>()
        );

        var cancellationToken = new CancellationToken();

        _mockSimulacaoService
            .Setup(x => x.ObterVolumeSimuladoAsync(It.IsAny<ObterVolumeSimuladoQuery>(), cancellationToken))
            .ReturnsAsync(volumeResult);

        // Act
        await _controller.ObterVolumePorDia(dataReferencia, cancellationToken);

        // Assert
        _mockSimulacaoService.Verify(
            x => x.ObterVolumeSimuladoAsync(It.IsAny<ObterVolumeSimuladoQuery>(), cancellationToken),
            Times.Once);
    }
}
