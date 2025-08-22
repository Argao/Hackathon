using Hackathon.Application.Services;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Tests.Services;

public class CachedProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _mockProdutoRepository;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<ILogger<CachedProdutoService>> _mockLogger;

    private readonly CachedProdutoService _service;

    public CachedProdutoServiceTests()
    {
        _mockProdutoRepository = new Mock<IProdutoRepository>();
        _mockCache = new Mock<IMemoryCache>();
        _mockLogger = new Mock<ILogger<CachedProdutoService>>();

        _service = new CachedProdutoService(_mockProdutoRepository.Object, _mockCache.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetProdutoAdequadoAsync_ComProdutoEncontrado_DeveRetornarProduto()
    {
        // Arrange
        var valor = ValorMonetario.Create(10000m).Value;
        var prazo = PrazoMeses.Create(12).Value;
        var ct = CancellationToken.None;

        var produtos = new List<Produto>
        {
            new Produto
            {
                Codigo = 1,
                Descricao = "Produto Teste",
                TaxaMensal = TaxaJuros.Create(0.015m).Value,
                MinMeses = 6,
                MaxMeses = 24,
                MinValor = ValorMonetario.Create(1000m).Value,
                MaxValor = ValorMonetario.Create(100000m).Value
            }
        };

        object? cachedValue = null;
        _mockCache.Setup(x => x.TryGetValue("produtos_all", out cachedValue))
            .Returns(false);

        _mockProdutoRepository.Setup(x => x.GetAllAsync(ct))
            .ReturnsAsync(produtos);

        // Act
        var result = await _service.GetProdutoAdequadoAsync(valor, prazo, ct);

        // Assert
        result.Should().NotBeNull();
        result!.Codigo.Should().Be(1);
        result.Descricao.Should().Be("Produto Teste");
        
        _mockProdutoRepository.Verify(x => x.GetAllAsync(ct), Times.Once);
    }

    [Fact]
    public async Task GetProdutoAdequadoAsync_ComProdutoNaoEncontrado_DeveRetornarNull()
    {
        // Arrange
        var valor = ValorMonetario.Create(1000000m).Value; // Valor muito alto
        var prazo = PrazoMeses.Create(12).Value;
        var ct = CancellationToken.None;

        var produtos = new List<Produto>
        {
            new Produto
            {
                Codigo = 1,
                Descricao = "Produto Teste",
                TaxaMensal = TaxaJuros.Create(0.015m).Value,
                MinMeses = 6,
                MaxMeses = 24,
                MinValor = ValorMonetario.Create(1000m).Value,
                MaxValor = ValorMonetario.Create(100000m).Value
            }
        };

        object? cachedValue = null;
        _mockCache.Setup(x => x.TryGetValue("produtos_all", out cachedValue))
            .Returns(false);

        _mockProdutoRepository.Setup(x => x.GetAllAsync(ct))
            .ReturnsAsync(produtos);

        // Act
        var result = await _service.GetProdutoAdequadoAsync(valor, prazo, ct);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProdutoAdequadoAsync_ComListaVazia_DeveRetornarNull()
    {
        // Arrange
        var valor = ValorMonetario.Create(10000m).Value;
        var prazo = PrazoMeses.Create(12).Value;
        var ct = CancellationToken.None;

        object? cachedValue = null;
        _mockCache.Setup(x => x.TryGetValue("produtos_all", out cachedValue))
            .Returns(false);

        _mockProdutoRepository.Setup(x => x.GetAllAsync(ct))
            .ReturnsAsync(new List<Produto>());

        // Act
        var result = await _service.GetProdutoAdequadoAsync(valor, prazo, ct);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ComCacheHit_DeveRetornarProdutosDoCache()
    {
        // Arrange
        var ct = CancellationToken.None;

        var produtos = new List<Produto>
        {
            new Produto { Codigo = 1, Descricao = "Produto 1" },
            new Produto { Codigo = 2, Descricao = "Produto 2" }
        };

        object? cachedValue = produtos;
        _mockCache.Setup(x => x.TryGetValue("produtos_all", out cachedValue))
            .Returns(true);

        // Act
        var result = await _service.GetAllAsync(ct);

        // Assert
        result.Should().NotBeNull();
        result!.Should().HaveCount(2);
        result!.First().Codigo.Should().Be(1);
        result!.Last().Codigo.Should().Be(2);
        
        // Não deve chamar o repository
        _mockProdutoRepository.Verify(x => x.GetAllAsync(ct), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ComRepositoryRetornandoNull_DeveRetornarNull()
    {
        // Arrange
        var ct = CancellationToken.None;

        object? cachedValue = null;
        _mockCache.Setup(x => x.TryGetValue("produtos_all", out cachedValue))
            .Returns(false);

        _mockProdutoRepository.Setup(x => x.GetAllAsync(ct))
            .ReturnsAsync((IEnumerable<Produto>?)null);

        // Act
        var result = await _service.GetAllAsync(ct);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void InvalidateCache_DeveRemoverCache()
    {
        // Act
        _service.InvalidateCache();

        // Assert
        _mockCache.Verify(x => x.Remove("produtos_all"), Times.Once);
    }

    [Fact]
    public async Task GetProdutoAdequadoAsync_ComProdutoQueAtendeRequisitos_DeveRetornarPrimeiroProdutoAdequado()
    {
        // Arrange
        var valor = ValorMonetario.Create(10000m).Value;
        var prazo = PrazoMeses.Create(12).Value;
        var ct = CancellationToken.None;

        var produtos = new List<Produto>
        {
            new Produto
            {
                Codigo = 1,
                Descricao = "Produto 1",
                TaxaMensal = TaxaJuros.Create(0.01m).Value,
                MinValor = ValorMonetario.Create(1000m).Value,
                MaxValor = ValorMonetario.Create(5000m).Value, // Não atende
                MinMeses = 6,
                MaxMeses = 60
            },
            new Produto
            {
                Codigo = 2,
                Descricao = "Produto 2",
                TaxaMensal = TaxaJuros.Create(0.01m).Value,
                MinValor = ValorMonetario.Create(1000m).Value,
                MaxValor = ValorMonetario.Create(100000m).Value, // Atende
                MinMeses = 6,
                MaxMeses = 60
            },
            new Produto
            {
                Codigo = 3,
                Descricao = "Produto 3",
                TaxaMensal = TaxaJuros.Create(0.01m).Value,
                MinValor = ValorMonetario.Create(1000m).Value,
                MaxValor = ValorMonetario.Create(200000m).Value, // Também atende
                MinMeses = 6,
                MaxMeses = 60
            }
        };

        object? cachedValue = null;
        _mockCache.Setup(x => x.TryGetValue("produtos_all", out cachedValue))
            .Returns(false);

        _mockProdutoRepository.Setup(x => x.GetAllAsync(ct))
            .ReturnsAsync(produtos);

        // Act
        var result = await _service.GetProdutoAdequadoAsync(valor, prazo, ct);

        // Assert
        result.Should().NotBeNull();
        result!.Codigo.Should().Be(2); // Primeiro produto que atende os requisitos
    }
}
