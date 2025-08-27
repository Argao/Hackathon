using Hackathon.Application.Services;
using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hackathon.Application.Tests.Services;

public class VolumeSimuladoCacheServiceTests
{
    private readonly IMemoryCache _cache;
    private readonly Mock<ISimulacaoRepository> _mockRepository;
    private readonly Mock<ILogger<VolumeSimuladoCacheService>> _mockLogger;
    private readonly VolumeSimuladoCacheService _cacheService;
    private readonly DateOnly _dataReferencia;

    public VolumeSimuladoCacheServiceTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _mockRepository = new Mock<ISimulacaoRepository>();
        _mockLogger = new Mock<ILogger<VolumeSimuladoCacheService>>();
        _cacheService = new VolumeSimuladoCacheService(_cache, _mockRepository.Object, _mockLogger.Object);
        _dataReferencia = DateOnly.FromDateTime(DateTime.Today);
    }

    [Fact]
    public async Task GetVolumeSimuladoAsync_QuandoCacheHit_DeveRetornarDadosDoCache()
    {
        // Arrange
        var dadosEsperados = new List<VolumeSimuladoProdutoDto>
        {
            new(1, "Produto 1", 0.05m, 1000m, 10000m, 12000m)
        };

        var cacheKey = $"volume_simulado_{_dataReferencia:yyyy-MM-dd}";
        _cache.Set(cacheKey, dadosEsperados);

        // Act
        var resultado = await _cacheService.GetVolumeSimuladoAsync(_dataReferencia);

        // Assert
        Assert.Equal(dadosEsperados, resultado);
        _mockRepository.Verify(x => x.ObterVolumeSimuladoPorProdutoAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetVolumeSimuladoAsync_QuandoCacheMiss_DeveBuscarDoRepositorio()
    {
        // Arrange
        var dadosEsperados = new List<VolumeSimuladoProdutoDto>
        {
            new(1, "Produto 1", 0.05m, 1000m, 10000m, 12000m)
        };

        _mockRepository.Setup(x => x.ObterVolumeSimuladoPorProdutoAsync(_dataReferencia, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(dadosEsperados);

        // Act
        var resultado = await _cacheService.GetVolumeSimuladoAsync(_dataReferencia);

        // Assert
        Assert.Equal(dadosEsperados, resultado);
        _mockRepository.Verify(x => x.ObterVolumeSimuladoPorProdutoAsync(_dataReferencia, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void InvalidateCache_DeveRemoverChaveDoCache()
    {
        // Arrange
        var cacheKey = $"volume_simulado_{_dataReferencia:yyyy-MM-dd}";
        _cache.Set(cacheKey, "teste");

        // Act
        _cacheService.InvalidateCache(_dataReferencia);

        // Assert
        Assert.False(_cache.TryGetValue(cacheKey, out _));
    }

    [Theory]
    [InlineData(0, 5)] // Hoje
    [InlineData(-1, 15)] // Ontem
    [InlineData(-7, 30)] // Última semana
    [InlineData(-30, 60)] // Histórico
    public async Task GetVolumeSimuladoAsync_DeveUsarTTLCorretoParaData(int diasOffset, int ttlMinutosEsperado)
    {
        // Arrange
        var dataTeste = DateOnly.FromDateTime(DateTime.Today.AddDays(diasOffset));
        var dadosEsperados = new List<VolumeSimuladoProdutoDto>
        {
            new(1, "Produto 1", 0.05m, 1000m, 10000m, 12000m)
        };

        _mockRepository.Setup(x => x.ObterVolumeSimuladoPorProdutoAsync(dataTeste, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(dadosEsperados);

        // Act
        await _cacheService.GetVolumeSimuladoAsync(dataTeste);

        // Assert
        _mockRepository.Verify(x => x.ObterVolumeSimuladoPorProdutoAsync(dataTeste, It.IsAny<CancellationToken>()), Times.Once);
    }
}
