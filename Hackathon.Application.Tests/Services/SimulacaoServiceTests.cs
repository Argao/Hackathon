using Hackathon.Application.Services;
using Hackathon.Application.Commands;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;
using Hackathon.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Hackathon.Domain.Exceptions;
using Hackathon.Domain.Enums;
using ValidationException = FluentValidation.ValidationException;

namespace Hackathon.Application.Tests.Services;

public class SimulacaoServiceTests
{
    private readonly Mock<ISimulacaoRepository> _mockSimulacaoRepository;
    private readonly Mock<ICachedProdutoService> _mockCachedProdutoService;
    private readonly Mock<IEventHubService> _mockEventHubService;
    private readonly Mock<ILogger<SimulacaoService>> _mockLogger;
    private readonly Mock<IValidator<RealizarSimulacaoCommand>> _mockSimulacaoValidator;
    private readonly Mock<IValidator<ListarSimulacoesQuery>> _mockListarValidator;
    private readonly Mock<IValidator<ObterVolumeSimuladoQuery>> _mockVolumeValidator;
    private readonly Mock<ICalculadoraAmortizacao> _mockCalculadoraPrice;
    private readonly Mock<ICalculadoraAmortizacao> _mockCalculadoraSac;
    private readonly List<ICalculadoraAmortizacao> _calculadoras;

    private readonly SimulacaoService _service;

    public SimulacaoServiceTests()
    {
        _mockSimulacaoRepository = new Mock<ISimulacaoRepository>();
        _mockCachedProdutoService = new Mock<ICachedProdutoService>();
        _mockEventHubService = new Mock<IEventHubService>();
        _mockLogger = new Mock<ILogger<SimulacaoService>>();
        _mockSimulacaoValidator = new Mock<IValidator<RealizarSimulacaoCommand>>();
        _mockListarValidator = new Mock<IValidator<ListarSimulacoesQuery>>();
        _mockVolumeValidator = new Mock<IValidator<ObterVolumeSimuladoQuery>>();

        // Mock das calculadoras
        _mockCalculadoraPrice = new Mock<ICalculadoraAmortizacao>();
        _mockCalculadoraSac = new Mock<ICalculadoraAmortizacao>();
        _calculadoras = new List<ICalculadoraAmortizacao> { _mockCalculadoraPrice.Object, _mockCalculadoraSac.Object };

        _service = new SimulacaoService(
            _mockCachedProdutoService.Object,
            _mockSimulacaoRepository.Object,
            _calculadoras,
            _mockSimulacaoValidator.Object,
            _mockListarValidator.Object,
            _mockVolumeValidator.Object,
            _mockEventHubService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComComandoValido_DeveRetornarSimulacaoResult()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult(); // Válido por padrão
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

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

        _mockCachedProdutoService.Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        var resultadoCalculo = new ResultadoSimulacao
        {
            Tipo = SistemaAmortizacao.PRICE,
            Parcelas = new List<Parcela>
            {
                new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(1000m).Value, ValorJuros = ValorMonetario.Create(100m).Value, ValorPrestacao = ValorMonetario.Create(1100m).Value }
            }
        };

        _mockCalculadoraPrice.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);
        _mockCalculadoraSac.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);

        var simulacao = new Simulacao
        {
            IdSimulacao = Guid.NewGuid(),
            CodigoProduto = 1,
            DescricaoProduto = "Produto Teste",
            TaxaJuros = TaxaJuros.Create(0.015m).Value,
            ValorDesejado = ValorMonetario.Create(10000m).Value,
            PrazoMeses = 12,
            Resultados = new List<ResultadoSimulacao> { resultadoCalculo }
        };

        _mockSimulacaoRepository.Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ReturnsAsync(simulacao);

        // Act
        var result = await _service.RealizarSimulacaoAsync(command, ct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CodigoProduto.Should().Be(1);
        result.DescricaoProduto.Should().Be("Produto Teste");
        result.Resultados.Should().HaveCount(2);
        
        _mockSimulacaoRepository.Verify(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct), Times.Once);
        _mockEventHubService.Verify(x => x.EnviarSimulacao(It.IsAny<SimulacaoResult>()), Times.Once);
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComValidacaoFalhando_DeveLancarValidationException()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(-1000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Valor", "Valor deve ser maior que zero"));
        
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var action = () => _service.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<Hackathon.Domain.Exceptions.ValidationException>()
            .WithMessage("*Valor deve ser maior que zero*");
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComValueObjectsInvalidos_DeveLancarValidationException()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(-1000m, 12); // Valor realmente inválido
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var action = () => _service.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<Hackathon.Domain.Exceptions.ValidationException>();
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComProdutoNaoEncontrado_DeveLancarSimulacaoException()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

        _mockCachedProdutoService.Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync((Produto?)null);

        // Act & Assert
        var action = () => _service.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<SimulacaoException>()
            .WithMessage("*Nenhum produto disponível*");
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComErroNaPersistencia_DeveLancarException()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

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

        _mockCachedProdutoService.Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        var resultadoCalculo = new ResultadoSimulacao
        {
            Tipo = SistemaAmortizacao.PRICE,
            Parcelas = new List<Parcela>
            {
                new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(1000m).Value, ValorJuros = ValorMonetario.Create(100m).Value, ValorPrestacao = ValorMonetario.Create(1100m).Value }
            }
        };

        _mockCalculadoraPrice.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);
        _mockCalculadoraSac.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);

        _mockSimulacaoRepository.Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ThrowsAsync(new Exception("Erro de banco"));

        // Act & Assert
        var action = () => _service.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Erro de banco");
    }

    [Fact]
    public async Task ListarSimulacoesAsync_ComQueryValida_DeveRetornarPagedResult()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(1, 10);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockListarValidator.Setup(x => x.ValidateAsync(query, ct))
            .ReturnsAsync(validationResult);

        var simulacoes = new List<Simulacao>
        {
            new Simulacao
            {
                IdSimulacao = Guid.NewGuid(),
                ValorDesejado = ValorMonetario.Create(10000m).Value,
                PrazoMeses = 12,
                Resultados = new List<ResultadoSimulacao>
                {
                    new ResultadoSimulacao
                    {
                        Parcelas = new List<Parcela>
                        {
                            new Parcela { ValorPrestacao = ValorMonetario.Create(1000m).Value }
                        }
                    }
                }
            }
        };

        _mockSimulacaoRepository.Setup(x => x.ObterTotalSimulacoesAsync(ct))
            .ReturnsAsync(1);
        _mockSimulacaoRepository.Setup(x => x.ListarSimulacoesAsync(1, 10, ct))
            .ReturnsAsync(simulacoes);

        // Act
        var result = await _service.ListarSimulacoesAsync(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.TotalItems.Should().Be(1);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task ListarSimulacoesAsync_ComValidacaoFalhando_DeveLancarValidationException()
    {
        // Arrange
        var query = new ListarSimulacoesQuery(0, 10); // Página inválida
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("NumeroPagina", "Número da página deve ser maior ou igual a 1"));
        
        _mockListarValidator.Setup(x => x.ValidateAsync(query, ct))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var action = () => _service.ListarSimulacoesAsync(query, ct);
        await action.Should().ThrowAsync<Hackathon.Domain.Exceptions.ValidationException>()
            .WithMessage("*Número da página deve ser maior ou igual a 1*");
    }

    [Fact]
    public async Task ObterVolumeSimuladoAsync_ComQueryValida_DeveRetornarVolumeSimuladoResult()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today));
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockVolumeValidator.Setup(x => x.ValidateAsync(query, ct))
            .ReturnsAsync(validationResult);

        var dadosAgregados = new List<VolumeSimuladoAgregado>
        {
            new VolumeSimuladoAgregado
            {
                CodigoProduto = 1,
                DescricaoProduto = "Produto Teste",
                TaxaMediaJuro = TaxaJuros.Create(0.015m).Value,
                ValorMedioPrestacao = ValorMonetario.Create(5000m).Value,
                ValorTotalDesejado = ValorMonetario.Create(100000m).Value,
                ValorTotalCredito = ValorMonetario.Create(120000m).Value
            }
        };

        _mockSimulacaoRepository.Setup(x => x.ObterVolumeSimuladoPorProdutoAsync(query.DataReferencia, ct))
            .ReturnsAsync(dadosAgregados);

        // Act
        var result = await _service.ObterVolumeSimuladoAsync(query, ct);

        // Assert
        result.Should().NotBeNull();
        result.DataReferencia.Should().Be(query.DataReferencia);
        result.Produtos.Should().HaveCount(1);
    }

    [Fact]
    public async Task ObterVolumeSimuladoAsync_ComValidacaoFalhando_DeveLancarValidationException()
    {
        // Arrange
        var query = new ObterVolumeSimuladoQuery(DateOnly.FromDateTime(DateTime.Today.AddDays(1))); // Data futura
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("DataReferencia", "Data de referência não pode ser futura"));
        
        _mockVolumeValidator.Setup(x => x.ValidateAsync(query, ct))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var action = () => _service.ObterVolumeSimuladoAsync(query, ct);
        await action.Should().ThrowAsync<Hackathon.Domain.Exceptions.ValidationException>()
            .WithMessage("*Data de referência não pode ser futura*");
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComFluxoCompletoSucesso_DeveExecutarTodasAsEtapas()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(15000m, 24);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

        var produto = new Produto
        {
            Codigo = 2,
            Descricao = "Produto Premium",
            TaxaMensal = TaxaJuros.Create(0.02m).Value,
            MinMeses = 12,
            MaxMeses = 36,
            MinValor = ValorMonetario.Create(5000m).Value,
            MaxValor = ValorMonetario.Create(200000m).Value
        };

        _mockCachedProdutoService.Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        // Mock das calculadoras com resultados mais realistas
        var resultadoPRICE = new ResultadoSimulacao
        {
            Tipo = SistemaAmortizacao.PRICE,
            Parcelas = new List<Parcela>
            {
                new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(500m).Value, ValorJuros = ValorMonetario.Create(300m).Value, ValorPrestacao = ValorMonetario.Create(800m).Value },
                new Parcela { Numero = 2, ValorAmortizacao = ValorMonetario.Create(510m).Value, ValorJuros = ValorMonetario.Create(290m).Value, ValorPrestacao = ValorMonetario.Create(800m).Value }
            }
        };

        var resultadoSAC = new ResultadoSimulacao
        {
            Tipo = SistemaAmortizacao.SAC,
            Parcelas = new List<Parcela>
            {
                new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(625m).Value, ValorJuros = ValorMonetario.Create(300m).Value, ValorPrestacao = ValorMonetario.Create(925m).Value },
                new Parcela { Numero = 2, ValorAmortizacao = ValorMonetario.Create(625m).Value, ValorJuros = ValorMonetario.Create(287.5m).Value, ValorPrestacao = ValorMonetario.Create(912.5m).Value }
            }
        };

        _mockCalculadoraPrice.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoPRICE);

        _mockCalculadoraSac.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoSAC);

        var simulacao = new Simulacao
        {
            IdSimulacao = Guid.NewGuid(),
            CodigoProduto = 2,
            DescricaoProduto = "Produto Premium",
            TaxaJuros = TaxaJuros.Create(0.02m).Value,
            ValorDesejado = ValorMonetario.Create(15000m).Value,
            PrazoMeses = 24,
            Resultados = new List<ResultadoSimulacao> { resultadoPRICE, resultadoSAC }
        };

        _mockSimulacaoRepository.Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ReturnsAsync(simulacao);

        // Act
        var result = await _service.RealizarSimulacaoAsync(command, ct);

        // Assert - Verificar resultado
        result.Should().NotBeNull();
        result.CodigoProduto.Should().Be(2);
        result.DescricaoProduto.Should().Be("Produto Premium");
        result.TaxaJuros.Should().Be(0.02m);
        result.Resultados.Should().HaveCount(2);
        
        var priceResult = result.Resultados.First(r => r.TipoAmortizacao == "PRICE");
        var sacResult = result.Resultados.First(r => r.TipoAmortizacao == "SAC");
        
        priceResult.Parcelas.Should().HaveCount(2);
        sacResult.Parcelas.Should().HaveCount(2);

        // Assert - Verificar que todas as etapas foram executadas
        _mockSimulacaoValidator.Verify(x => x.ValidateAsync(command, ct), Times.Once);
        _mockCachedProdutoService.Verify(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct), Times.Once);
        _mockCalculadoraPrice.Verify(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()), Times.Once);
        _mockCalculadoraSac.Verify(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()), Times.Once);
        _mockSimulacaoRepository.Verify(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct), Times.Once);
        _mockEventHubService.Verify(x => x.EnviarSimulacao(It.IsAny<SimulacaoResult>()), Times.Once);
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_ComErroPersistencia_DevePropagrarExcecao()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

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

        _mockCachedProdutoService.Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        var resultadoCalculo = new ResultadoSimulacao
        {
            Tipo = SistemaAmortizacao.PRICE,
            Parcelas = new List<Parcela>
            {
                new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(1000m).Value, ValorJuros = ValorMonetario.Create(100m).Value, ValorPrestacao = ValorMonetario.Create(1100m).Value }
            }
        };

        _mockCalculadoraPrice.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);
        _mockCalculadoraSac.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);

        _mockSimulacaoRepository.Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ThrowsAsync(new InvalidOperationException("Erro crítico na persistência"));

        // Act & Assert
        var action = () => _service.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro crítico na persistência");

        // Verificar que tentou persistir
        _mockSimulacaoRepository.Verify(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct), Times.Once);
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_QuandoEventHubFalha_DeveLogarErroMasNaoFalhar()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

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

        _mockCachedProdutoService.Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        var resultadoCalculo = new ResultadoSimulacao
        {
            Tipo = SistemaAmortizacao.PRICE,
            Parcelas = new List<Parcela>
            {
                new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(1000m).Value, ValorJuros = ValorMonetario.Create(100m).Value, ValorPrestacao = ValorMonetario.Create(1100m).Value }
            }
        };

        _mockCalculadoraPrice.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);
        _mockCalculadoraSac.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);

        var simulacao = new Simulacao
        {
            IdSimulacao = Guid.NewGuid(),
            CodigoProduto = 1,
            DescricaoProduto = "Produto Teste",
            TaxaJuros = TaxaJuros.Create(0.015m).Value,
            Resultados = new List<ResultadoSimulacao> { resultadoCalculo }
        };

        _mockSimulacaoRepository.Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ReturnsAsync(simulacao);

        // Configurar EventHub para falhar
        _mockEventHubService.Setup(x => x.EnviarSimulacao(It.IsAny<SimulacaoResult>()))
            .Throws(new Exception("EventHub error"));

        // Act
        var result = await _service.RealizarSimulacaoAsync(command, ct);

        // Assert
        result.Should().NotBeNull();
        
        // Aguardar um pouco para a thread de background executar
        await Task.Delay(100, ct);
        
        // Verificar se o erro foi logado
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RealizarSimulacaoAsync_QuandoRepositoryFalha_DeveRetornarErro()
    {
        // Arrange
        var command = new RealizarSimulacaoCommand(10000m, 12);
        var ct = CancellationToken.None;

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockSimulacaoValidator.Setup(x => x.ValidateAsync(command, ct))
            .ReturnsAsync(validationResult);

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

        _mockCachedProdutoService.Setup(x => x.GetProdutoAdequadoAsync(It.IsAny<ValorMonetario>(), It.IsAny<PrazoMeses>(), ct))
            .ReturnsAsync(produto);

        var resultadoCalculo = new ResultadoSimulacao
        {
            Tipo = SistemaAmortizacao.PRICE,
            Parcelas = new List<Parcela>
            {
                new Parcela { Numero = 1, ValorAmortizacao = ValorMonetario.Create(1000m).Value, ValorJuros = ValorMonetario.Create(100m).Value, ValorPrestacao = ValorMonetario.Create(1100m).Value }
            }
        };

        _mockCalculadoraPrice.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);
        _mockCalculadoraSac.Setup(x => x.Calcular(It.IsAny<ValorMonetario>(), It.IsAny<TaxaJuros>(), It.IsAny<PrazoMeses>()))
            .Returns(resultadoCalculo);

        // Configurar repository para falhar
        _mockSimulacaoRepository.Setup(x => x.AdicionarAsync(It.IsAny<Simulacao>(), ct))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var action = () => _service.RealizarSimulacaoAsync(command, ct);
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Database error");
    }
}
