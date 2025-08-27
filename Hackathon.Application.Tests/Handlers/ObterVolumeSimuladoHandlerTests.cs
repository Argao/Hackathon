using Hackathon.Application.Handlers;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Interfaces.Repositories;
using Moq;

namespace Hackathon.Application.Tests.Handlers;

public class ObterVolumeSimuladoHandlerTests
{
    private readonly Mock<ISimulacaoRepository> _mockRepository;
    private readonly ObterVolumeSimuladoHandler _handler;

    public ObterVolumeSimuladoHandlerTests()
    {
        _mockRepository = new Mock<ISimulacaoRepository>();
        _handler = new ObterVolumeSimuladoHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ComQueryValida_DeveRetornarVolumeSimuladoResult()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterVolumeSimuladoQuery(dataReferencia);
        var ct = CancellationToken.None;

        var dadosAgregados = new List<VolumeSimuladoProdutoDto>
        {
            new VolumeSimuladoProdutoDto(
                CodigoProduto: 1,
                DescricaoProduto: "Produto Teste",
                TaxaMediaJuro: 0.015m,
                ValorMedioPrestacao: 1000m,
                ValorTotalDesejado: 10000m,
                ValorTotalCredito: 12000m
            )
        };

        _mockRepository
            .Setup(x => x.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, ct))
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _handler.Handle(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.DataReferencia.Should().Be(dataReferencia);
        result.Produtos.Should().HaveCount(1);

        var produto = result.Produtos.First();
        produto.CodigoProduto.Should().Be(1);
        produto.DescricaoProduto.Should().Be("Produto Teste");
        produto.TaxaMediaJuro.Should().Be(0.015m);
        produto.ValorTotalDesejado.Should().Be(10000m);
        produto.ValorTotalCredito.Should().Be(12000m);

        _mockRepository.Verify(
            x => x.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ComDadosVazios_DeveRetornarResultadoVazio()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterVolumeSimuladoQuery(dataReferencia);
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(x => x.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, ct))
            .ReturnsAsync(new List<VolumeSimuladoProdutoDto>());

        // Act
        var result = await _handler.Handle(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.DataReferencia.Should().Be(dataReferencia);
        result.Produtos.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ComRepositoryLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterVolumeSimuladoQuery(dataReferencia);
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(x => x.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, ct))
            .ThrowsAsync(new Exception("Erro no repositório"));

        // Act & Assert
        var action = () => _handler.Handle(query, ct);
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Erro no repositório");
    }

    [Fact]
    public async Task Handle_ComCancellationTokenCancelado_DevePropagarCancellationToken()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var query = new ObterVolumeSimuladoQuery(dataReferencia);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockRepository
            .Setup(x => x.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, cts.Token))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var action = () => _handler.Handle(query, cts.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();

        _mockRepository.Verify(x => x.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, cts.Token), Times.Once);
    }
}
