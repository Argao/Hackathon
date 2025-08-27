using Hackathon.Application.Commands;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Results;
using Hackathon.Application.Services;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Enums;
using Hackathon.Domain.Exceptions;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Application.Tests.Services;

public class SimulacaoOrchestratorTests
{
    private readonly Mock<ICachedProdutoService> _mockProdutoService;
    private readonly Mock<ISimulacaoFactory> _mockSimulacaoFactory;
    private readonly Mock<ICalculadoraService> _mockCalculadoraService;
    private readonly Mock<ISimulacaoRepository> _mockRepository;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<SimulacaoOrchestrator>> _mockLogger;
    private readonly IMemoryCache _cache;
    private readonly SimulacaoOrchestrator _orchestrator;

    public SimulacaoOrchestratorTests()
    {
        _mockProdutoService = new Mock<ICachedProdutoService>();
        _mockSimulacaoFactory = new Mock<ISimulacaoFactory>();
        _mockCalculadoraService = new Mock<ICalculadoraService>();
        _mockRepository = new Mock<ISimulacaoRepository>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<SimulacaoOrchestrator>>();
        
        // ✅ OTIMIZAÇÃO: Usar cache real para testes
        _cache = new MemoryCache(new MemoryCacheOptions());
        
        _orchestrator = new SimulacaoOrchestrator(
            _mockProdutoService.Object,
            _mockSimulacaoFactory.Object,
            _mockCalculadoraService.Object,
            _mockRepository.Object,
            _mockEventPublisher.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _cache
        );
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComComandoValido_DeveRetornarSimulacaoResult()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var produto = new Produto
        {
            Codigo = 1,
            Descricao = "Produto Teste",
            TaxaMensal = TaxaJuros.Create(0.015m).Value,
            MinMeses = 6,
            MaxMeses = 24,
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(100000m).Value
        };

        var simulacao = Simulacao.Create(
            1,
            "Produto Teste",
            TaxaJuros.Create(0.015m).Value,
            ValorMonetario.Create(10000m).Value,
            PrazoMeses.Create(12).Value,
            DateOnly.FromDateTime(DateTime.Today)
        );

        var resultados = new List<ResultadoSimulacao>
        {
            new ResultadoSimulacao
            {
                Tipo = SistemaAmortizacao.PRICE,
                Parcelas = new List<Parcela>
                {
                    new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(1000m).Value, ValorJuros = ValorMonetario.Create(100m).Value, ValorPrestacao = ValorMonetario.Create(1100m).Value }
                }
            }
        };

        var simulacaoResult = new SimulacaoResult(
            Guid.NewGuid(),
            1,
            "Produto Teste",
            0.015m,
            new List<ResultadoCalculoAmortizacao>()
        );

        _mockProdutoService
            .Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        _mockSimulacaoFactory
            .Setup(x => x.CriarSimulacao(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TaxaJuros>(), It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>()))
            .Returns(simulacao);

        _mockCalculadoraService
            .Setup(x => x.ExecutarCalculos(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultados);

        _mockRepository
            .Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ReturnsAsync(simulacao);

        _mockMapper
            .Setup(x => x.Map<Domain.Entities.Simulacao, SimulacaoResult>(simulacao))
            .Returns(simulacaoResult);

        // Act
        var result = await _orchestrator.RealizarSimulacaoAsync(command, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(simulacaoResult);

        _mockProdutoService.Verify(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct), Times.Once);
        _mockSimulacaoFactory.Verify(x => x.CriarSimulacao(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TaxaJuros>(), It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>()), Times.Once);
        _mockCalculadoraService.Verify(x => x.ExecutarCalculos(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()), Times.Once);
        _mockRepository.Verify(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<SimulacaoResult>()), Times.Once);
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComCacheHit_DeveRetornarResultadoCacheado()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var simulacaoResult = new SimulacaoResult(
            Guid.NewGuid(),
            1,
            "Produto Teste",
            0.015m,
            new List<ResultadoCalculoAmortizacao>()
        );

        // ✅ OTIMIZAÇÃO: Adicionar resultado no cache real
        var cacheKey = $"simulacao_{command.Valor}_{command.Prazo}";
        _cache.Set(cacheKey, simulacaoResult, TimeSpan.FromMinutes(30));

        // Act
        var result = await _orchestrator.RealizarSimulacaoAsync(command, ct);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(simulacaoResult);

        // Verificar que usou cache e não executou cálculos
        _mockProdutoService.Verify(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct), Times.Never);
        _mockSimulacaoFactory.Verify(x => x.CriarSimulacao(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TaxaJuros>(), It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>()), Times.Never);
        _mockCalculadoraService.Verify(x => x.ExecutarCalculos(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()), Times.Never);
        _mockRepository.Verify(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct), Times.Never);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<SimulacaoResult>()), Times.Never);
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComValueObjectsInvalidos_DeveLancarValidationException()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(-1000m, 12);
        var ct = CancellationToken.None;

        // Act & Assert
        var action = () => _orchestrator.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComProdutoNaoEncontrado_DeveLancarSimulacaoException()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        _mockProdutoService
            .Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync((Produto?)null);

        // Act & Assert
        var action = () => _orchestrator.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<SimulacaoException>()
            .WithMessage($"Nenhum produto disponível para valor {command.Valor} e prazo {command.Prazo}");
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComErroNoRepository_DevePropagarExcecao()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var produto = new Produto
        {
            Codigo = 1,
            Descricao = "Produto Teste",
            TaxaMensal = TaxaJuros.Create(0.015m).Value,
            MinMeses = 6,
            MaxMeses = 24,
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(100000m).Value
        };

        var simulacao = Simulacao.Create(
            1,
            "Produto Teste",
            TaxaJuros.Create(0.015m).Value,
            ValorMonetario.Create(10000m).Value,
            PrazoMeses.Create(12).Value,
            DateOnly.FromDateTime(DateTime.Today)
        );

        var resultados = new List<ResultadoSimulacao>
        {
            new ResultadoSimulacao
            {
                Tipo = SistemaAmortizacao.PRICE,
                Parcelas = new List<Parcela>
                {
                    new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(1000m).Value, ValorJuros = ValorMonetario.Create(100m).Value, ValorPrestacao = ValorMonetario.Create(1100m).Value }
                }
            }
        };

        var simulacaoResult = new SimulacaoResult(
            Guid.NewGuid(),
            1,
            "Produto Teste",
            0.015m,
            new List<ResultadoCalculoAmortizacao>()
        );

        _mockProdutoService
            .Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        _mockSimulacaoFactory
            .Setup(x => x.CriarSimulacao(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TaxaJuros>(), It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>()))
            .Returns(simulacao);

        _mockCalculadoraService
            .Setup(x => x.ExecutarCalculos(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultados);

        _mockRepository
            .Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ThrowsAsync(new InvalidOperationException("Erro no repositório"));

        _mockMapper
            .Setup(x => x.Map<Domain.Entities.Simulacao, SimulacaoResult>(simulacao))
            .Returns(simulacaoResult);

        // Act & Assert
        var action = () => _orchestrator.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro no repositório");
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComErroNoEventPublisher_DevePropagarExcecao()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var produto = new Produto
        {
            Codigo = 1,
            Descricao = "Produto Teste",
            TaxaMensal = TaxaJuros.Create(0.015m).Value,
            MinMeses = 6,
            MaxMeses = 24,
            MinValor = ValorMonetario.Create(1000m).Value,
            MaxValor = ValorMonetario.Create(100000m).Value
        };

        var simulacao = Simulacao.Create(
            1,
            "Produto Teste",
            TaxaJuros.Create(0.015m).Value,
            ValorMonetario.Create(10000m).Value,
            PrazoMeses.Create(12).Value,
            DateOnly.FromDateTime(DateTime.Today)
        );

        var resultados = new List<ResultadoSimulacao>
        {
            new ResultadoSimulacao
            {
                Tipo = SistemaAmortizacao.PRICE,
                Parcelas = new List<Parcela>
                {
                    new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(1000m).Value, ValorJuros = ValorMonetario.Create(100m).Value, ValorPrestacao = ValorMonetario.Create(1100m).Value }
                }
            }
        };

        var simulacaoResult = new SimulacaoResult(
            Guid.NewGuid(),
            1,
            "Produto Teste",
            0.015m,
            new List<ResultadoCalculoAmortizacao>()
        );

        _mockProdutoService
            .Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        _mockSimulacaoFactory
            .Setup(x => x.CriarSimulacao(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TaxaJuros>(), It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>()))
            .Returns(simulacao);

        _mockCalculadoraService
            .Setup(x => x.ExecutarCalculos(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultados);

        _mockRepository
            .Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ReturnsAsync(simulacao);

        _mockMapper
            .Setup(x => x.Map<Domain.Entities.Simulacao, SimulacaoResult>(simulacao))
            .Returns(simulacaoResult);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<SimulacaoResult>()))
            .Throws(new InvalidOperationException("Erro no event publisher"));

        // Act & Assert
        var action = () => _orchestrator.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro no event publisher");
    }
}
