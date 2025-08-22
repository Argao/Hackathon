using Microsoft.EntityFrameworkCore;

namespace Hackathon.Infrastructure.Tests.Repositories;

public class MetricaRepositoryTests
{
    [Fact]
    public void Constructor_ComParametrosValidos_DeveCriarInstancia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();

        // Act & Assert
        FluentActions.Invoking(() => new MetricaRepository(context, logger.Object))
            .Should().NotThrow();
    }

    [Fact]
    public async Task SalvarMetricaAsync_ComMetricaValida_DeveSalvarComSucesso()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();
        var repository = new MetricaRepository(context, logger.Object);
        var metrica = CreateMetricaRequisicao();

        // Act
        await repository.SalvarMetricaAsync(metrica, CancellationToken.None);

        // Assert
        var metricaSalva = await context.Metricas.FirstOrDefaultAsync();
        metricaSalva.Should().NotBeNull();
        metricaSalva!.NomeApi.Should().Be(metrica.NomeApi);
        metricaSalva.Endpoint.Should().Be(metrica.Endpoint);
    }

    [Fact]
    public async Task ObterMetricasPorDataAsync_ComDados_DeveRetornarMetricasAgregadas()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();
        var repository = new MetricaRepository(context, logger.Object);
        
        // Adicionar dados de teste
        var metricas = CreateListaMetricas();
        context.Metricas.AddRange(metricas);
        await context.SaveChangesAsync();

        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var resultado = await repository.ObterMetricasPorDataAsync(dataReferencia, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeOfType<List<MetricaAgregada>>();
        resultado.Should().NotBeEmpty();
        
        var primeiraMetrica = resultado.First();
        primeiraMetrica.NomeApi.Should().Be("API.Teste");
        primeiraMetrica.QtdRequisicoes.Should().BeGreaterThan(0);
        primeiraMetrica.TempoMedio.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ObterMetricasPorDataAsync_SemDados_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();
        var repository = new MetricaRepository(context, logger.Object);

        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var resultado = await repository.ObterMetricasPorDataAsync(dataReferencia, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterMetricasPorDataAsync_ComDataDiferente_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();
        var repository = new MetricaRepository(context, logger.Object);
        
        // Adicionar dados de teste
        var metricas = CreateListaMetricas();
        context.Metricas.AddRange(metricas);
        await context.SaveChangesAsync();

        var dataReferencia = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)); // Data diferente

        // Act
        var resultado = await repository.ObterMetricasPorDataAsync(dataReferencia, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterMetricasPorDataAsync_ComExcecao_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();
        var repository = new MetricaRepository(context, logger.Object);
        
        // Dispose do contexto para forçar erro
        context.Dispose();
        
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var resultado = await repository.ObterMetricasPorDataAsync(dataReferencia, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
        
        // Verificar se o erro foi logado
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SalvarMetricaAsync_ComExcecaoNoContexto_NaoDeveLancarExcecao()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();
        var repository = new MetricaRepository(context, logger.Object);
        
        // Dispose do contexto para forçar erro
        context.Dispose();
        
        var metrica = CreateMetricaRequisicao();

        // Act & Assert
        await FluentActions.Invoking(() => repository.SalvarMetricaAsync(metrica, CancellationToken.None))
            .Should().NotThrowAsync();

        // Verificar se o erro foi logado
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SalvarMetricaAsync_ComOperationCanceledException_DeveLogarDebug()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();
        var repository = new MetricaRepository(context, logger.Object);
        
        var metrica = CreateMetricaRequisicao();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        await repository.SalvarMetricaAsync(metrica, cts.Token);

        // Assert
        // Verificar se o debug foi logado
        logger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SalvarMetricaAsync_ComSucesso_DeveLogarDebug()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var logger = new Mock<ILogger<MetricaRepository>>();
        var repository = new MetricaRepository(context, logger.Object);
        var metrica = CreateMetricaRequisicao();

        // Act
        await repository.SalvarMetricaAsync(metrica, CancellationToken.None);

        // Assert
        // Verificar se o debug foi logado
        logger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static MetricaRequisicao CreateMetricaRequisicao()
    {
        return new MetricaRequisicao
        {
            NomeApi = "API.Teste",
            Endpoint = "GET /api/test",
            StatusCode = 200,
            TempoRespostaMs = 150,
            Sucesso = true,
            DataHora = DateTime.UtcNow
        };
    }

    private static List<MetricaRequisicao> CreateListaMetricas()
    {
        return new List<MetricaRequisicao>
        {
            new() { NomeApi = "API.Teste", Endpoint = "POST /api/simulacao", StatusCode = 200, TempoRespostaMs = 100, Sucesso = true, DataHora = DateTime.UtcNow },
            new() { NomeApi = "API.Teste", Endpoint = "POST /api/simulacao", StatusCode = 200, TempoRespostaMs = 150, Sucesso = true, DataHora = DateTime.UtcNow },
            new() { NomeApi = "API.Teste", Endpoint = "GET /api/produtos", StatusCode = 200, TempoRespostaMs = 50, Sucesso = true, DataHora = DateTime.UtcNow },
            new() { NomeApi = "API.Teste", Endpoint = "POST /api/simulacao", StatusCode = 400, TempoRespostaMs = 200, Sucesso = false, DataHora = DateTime.UtcNow }
        };
    }
}