using Hackathon.Application.Handlers;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Application.Services;
using Hackathon.Domain.Interfaces.Repositories;
using Mapster;
using MediatR;
using Moq;

namespace Hackathon.Application.Tests.Handlers;

public class ObterVolumeSimuladoHandlerTests
{
    private readonly Mock<IVolumeSimuladoCacheService> _mockCacheService;
    private readonly ObterVolumeSimuladoHandler _handler;
    private readonly DateOnly _dataReferencia;

    public ObterVolumeSimuladoHandlerTests()
    {
        _mockCacheService = new Mock<IVolumeSimuladoCacheService>();
        _handler = new ObterVolumeSimuladoHandler(_mockCacheService.Object);
        _dataReferencia = DateOnly.FromDateTime(DateTime.Today);
    }

    [Fact]
    public async Task Handle_ComDataValida_DeveRetornarVolumeSimuladoResult()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(_dataReferencia);
        
        var dadosAgregados = new List<VolumeSimuladoProdutoDto>
        {
            new(1, "Produto 1", 0.05m, 1000m, 10000m, 12000m),
            new(2, "Produto 2", 0.06m, 1500m, 15000m, 18000m)
        };

        _mockCacheService
            .Setup(x => x.GetVolumeSimuladoAsync(_dataReferencia, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DataReferencia.Should().Be(_dataReferencia);
        result.Produtos.Should().HaveCount(2);
        
        var primeiroProduto = result.Produtos.First();
        primeiroProduto.CodigoProduto.Should().Be(1);
        primeiroProduto.DescricaoProduto.Should().Be("Produto 1");
        primeiroProduto.TaxaMediaJuro.Should().Be(0.05m);
        primeiroProduto.ValorMedioPrestacao.Should().Be(1000m);
        primeiroProduto.ValorTotalDesejado.Should().Be(10000m);
        primeiroProduto.ValorTotalCredito.Should().Be(12000m);

        _mockCacheService.Verify(
            x => x.GetVolumeSimuladoAsync(_dataReferencia, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComListaVazia_DeveRetornarResultadoVazio()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(_dataReferencia);
        
        _mockCacheService
            .Setup(x => x.GetVolumeSimuladoAsync(_dataReferencia, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<VolumeSimuladoProdutoDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DataReferencia.Should().Be(_dataReferencia);
        result.Produtos.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ComCancellationToken_DevePassarTokenParaServico()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(_dataReferencia);
        var cancellationToken = new CancellationToken();
        
        _mockCacheService
            .Setup(x => x.GetVolumeSimuladoAsync(_dataReferencia, cancellationToken))
            .ReturnsAsync(new List<VolumeSimuladoProdutoDto>());

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _mockCacheService.Verify(
            x => x.GetVolumeSimuladoAsync(_dataReferencia, cancellationToken),
            Times.Once);
    }
}
