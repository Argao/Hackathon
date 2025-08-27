using Hackathon.Application.Handlers;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Interfaces.Repositories;
using Moq;

namespace Hackathon.Application.Tests.Handlers;

public class ListarSimulacoesHandlerTests
{
    private readonly Mock<ISimulacaoRepository> _mockRepository;
    private readonly ListarSimulacoesHandler _handler;

    public ListarSimulacoesHandlerTests()
    {
        _mockRepository = new Mock<ISimulacaoRepository>();
        _handler = new ListarSimulacoesHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ComQueryValida_DeveRetornarPagedResult()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 10);
        var ct = CancellationToken.None;

        var simulacoesDto = new List<SimulacaoResumoDto>
        {
            new(Guid.NewGuid(), 10000m, 12, 12000m)
        };

        _mockRepository
            .Setup(x => x.ObterTotalSimulacoesAsync(ct))
            .ReturnsAsync(1);

        _mockRepository
            .Setup(x => x.ListarSimulacoesOtimizadoAsync(1, 10, ct))
            .ReturnsAsync(simulacoesDto);

        // Act
        var result = await _handler.Handle(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.TotalItems.Should().Be(1);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);

        _mockRepository.Verify(x => x.ObterTotalSimulacoesAsync(ct), Times.Once);
        _mockRepository.Verify(x => x.ListarSimulacoesOtimizadoAsync(1, 10, ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComQueryComPaginaInvalida_DeveUsarPaginaPadrao()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(0, 10);
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(x => x.ObterTotalSimulacoesAsync(ct))
            .ReturnsAsync(0);

        _mockRepository
            .Setup(x => x.ListarSimulacoesOtimizadoAsync(1, 10, ct))
            .ReturnsAsync(new List<SimulacaoResumoDto>());

        // Act
        var result = await _handler.Handle(query, ct);

        // Assert
        result.CurrentPage.Should().Be(1);
        _mockRepository.Verify(x => x.ListarSimulacoesOtimizadoAsync(1, 10, ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComQueryComTamanhoPaginaInvalido_DeveUsarTamanhoPadrao()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 0);
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(x => x.ObterTotalSimulacoesAsync(ct))
            .ReturnsAsync(0);

        _mockRepository
            .Setup(x => x.ListarSimulacoesOtimizadoAsync(1, 1, ct))
            .ReturnsAsync(new List<SimulacaoResumoDto>());

        // Act
        var result = await _handler.Handle(query, ct);

        // Assert
        result.PageSize.Should().Be(1);
        _mockRepository.Verify(x => x.ListarSimulacoesOtimizadoAsync(1, 1, ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ComRepositoryLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 10);
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(x => x.ObterTotalSimulacoesAsync(ct))
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
        var query = new ListarSimulacoesQuery(1, 10);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockRepository
            .Setup(x => x.ObterTotalSimulacoesAsync(cts.Token))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var action = () => _handler.Handle(query, cts.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();

        _mockRepository.Verify(x => x.ObterTotalSimulacoesAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task Handle_ComExecucaoParalela_DeveExecutarConsultasEmParalelo()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 10);
        var ct = CancellationToken.None;

        var simulacoesDto = new List<SimulacaoResumoDto>
        {
            new(Guid.NewGuid(), 10000m, 12, 12000m)
        };

        _mockRepository
            .Setup(x => x.ObterTotalSimulacoesAsync(ct))
            .ReturnsAsync(1);

        _mockRepository
            .Setup(x => x.ListarSimulacoesOtimizadoAsync(1, 10, ct))
            .ReturnsAsync(simulacoesDto);

        // Act
        var result = await _handler.Handle(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        
        // Verificar que ambas as consultas foram chamadas
        _mockRepository.Verify(x => x.ObterTotalSimulacoesAsync(ct), Times.Once);
        _mockRepository.Verify(x => x.ListarSimulacoesOtimizadoAsync(1, 10, ct), Times.Once);
    }
}
