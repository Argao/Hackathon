using Microsoft.EntityFrameworkCore;
using Hackathon.Domain.Enums;

namespace Hackathon.Infrastructure.Tests.Repositories;

public class SimulacaoRepositoryTests
{
    [Fact]
    public void Constructor_ComContextoValido_DeveCriarInstancia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        using var context = new AppDbContext(options);

        // Act & Assert
        FluentActions.Invoking(() => new SimulacaoRepository(context))
            .Should().NotThrow();
    }

    [Fact]
    public async Task AdicionarAsync_ComSimulacaoValida_DeveAdicionarComSucesso()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);
        var simulacao = CreateSimulacao();

        // Act
        var resultado = await repository.AdicionarAsync(simulacao, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.IdSimulacao.Should().NotBe(Guid.Empty);
        
        // Verificar se foi salvo no contexto
        var simulacaoSalva = await context.Simulacoes.FirstOrDefaultAsync();
        simulacaoSalva.Should().NotBeNull();
        simulacaoSalva!.CodigoProduto.Should().Be(simulacao.CodigoProduto);
    }

    [Fact]
    public async Task ListarPaginadoAsync_ComDados_DeveRetornarResultadoPaginado()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Adicionar dados de teste
        var simulacoes = CreateListaSimulacoes();
        context.Simulacoes.AddRange(simulacoes);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ListarPaginadoAsync(1, 10, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Data.Should().NotBeNull();
        resultado.TotalRecords.Should().Be(simulacoes.Count);
    }

    [Fact]
    public async Task ListarPaginadoAsync_ComPaginaVazia_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Act
        var resultado = await repository.ListarPaginadoAsync(1, 10, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Data.Should().BeEmpty();
        resultado.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task ListarPaginadoAsync_ComPaginaMaiorQueTotal_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Adicionar dados de teste
        var simulacoes = CreateListaSimulacoes();
        context.Simulacoes.AddRange(simulacoes);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ListarPaginadoAsync(10, 10, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Data.Should().BeEmpty();
        resultado.TotalRecords.Should().Be(simulacoes.Count);
    }

    [Fact]
    public async Task ObterTotalSimulacoesAsync_ComDados_DeveRetornarTotalCorreto()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Adicionar dados de teste
        var simulacoes = CreateListaSimulacoes();
        context.Simulacoes.AddRange(simulacoes);
        await context.SaveChangesAsync();

        // Act
        var total = await repository.ObterTotalSimulacoesAsync(CancellationToken.None);

        // Assert
        total.Should().Be(simulacoes.Count);
    }

    [Fact]
    public async Task ObterTotalSimulacoesAsync_SemDados_DeveRetornarZero()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Act
        var total = await repository.ObterTotalSimulacoesAsync(CancellationToken.None);

        // Assert
        total.Should().Be(0);
    }

    [Fact]
    public async Task ObterVolumeSimuladoPorProdutoAsync_ComDados_DeveRetornarAgregacaoCorreta()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Adicionar dados de teste com resultados e parcelas
        var simulacoes = CreateSimulacoesComResultados();
        context.Simulacoes.AddRange(simulacoes);
        await context.SaveChangesAsync();

        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var resultado = await repository.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().NotBeEmpty();
        
        var primeiroResultado = resultado.First();
        primeiroResultado.CodigoProduto.Should().Be(1);
        primeiroResultado.DescricaoProduto.Should().Be("Produto 1");
        primeiroResultado.TaxaMediaJuro.Should().NotBeNull();
        primeiroResultado.ValorTotalDesejado.Should().NotBeNull();
        primeiroResultado.ValorTotalCredito.Should().NotBeNull();
    }

    [Fact]
    public async Task ObterVolumeSimuladoPorProdutoAsync_SemDados_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var resultado = await repository.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterVolumeSimuladoPorProdutoAsync_ComDataDiferente_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Adicionar dados de teste
        var simulacoes = CreateSimulacoesComResultados();
        context.Simulacoes.AddRange(simulacoes);
        await context.SaveChangesAsync();

        var dataReferencia = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)); // Data diferente

        // Act
        var resultado = await repository.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ListarSimulacoesAsync_ComDados_DeveRetornarSimulacoesPaginadas()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Adicionar dados de teste
        var simulacoes = CreateSimulacoesComResultados();
        context.Simulacoes.AddRange(simulacoes);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ListarSimulacoesAsync(1, 10, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().NotBeEmpty();
        resultado.Count().Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public async Task ListarSimulacoesAsync_SemDados_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Act
        var resultado = await repository.ListarSimulacoesAsync(1, 10, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ListarSimulacoesAsync_ComPaginaMaiorQueTotal_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var repository = new SimulacaoRepository(context);

        // Adicionar dados de teste
        var simulacoes = CreateSimulacoesComResultados();
        context.Simulacoes.AddRange(simulacoes);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ListarSimulacoesAsync(10, 10, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    private static Simulacao CreateSimulacao()
    {
        var taxaResult = TaxaJuros.Create(0.015m);
        var valorResult = ValorMonetario.Create(10000m);
        
        return new Simulacao
        {
            CodigoProduto = 1,
            DescricaoProduto = "Produto Teste",
            TaxaJuros = taxaResult.Value,
            PrazoMeses = 12,
            ValorDesejado = valorResult.Value,
            DataReferencia = DateOnly.FromDateTime(DateTime.Today)
        };
    }

    private static List<Simulacao> CreateListaSimulacoes()
    {
        var taxaResult = TaxaJuros.Create(0.015m);
        var valorResult = ValorMonetario.Create(10000m);
        
        return new List<Simulacao>
        {
            new()
            {
                CodigoProduto = 1,
                DescricaoProduto = "Produto 1",
                TaxaJuros = taxaResult.Value,
                PrazoMeses = 12,
                ValorDesejado = valorResult.Value,
                DataReferencia = DateOnly.FromDateTime(DateTime.Today)
            },
            new()
            {
                CodigoProduto = 2,
                DescricaoProduto = "Produto 2",
                TaxaJuros = taxaResult.Value,
                PrazoMeses = 24,
                ValorDesejado = valorResult.Value,
                DataReferencia = DateOnly.FromDateTime(DateTime.Today)
            }
        };
    }

    private static List<Simulacao> CreateSimulacoesComResultados()
    {
        var taxaResult = TaxaJuros.Create(0.015m);
        var valorResult = ValorMonetario.Create(10000m);
        var prazoResult = PrazoMeses.Create(12);
        
        var simulacoes = new List<Simulacao>();
        
        for (int i = 1; i <= 3; i++)
        {
            var simulacao = new Simulacao
            {
                CodigoProduto = i,
                DescricaoProduto = $"Produto {i}",
                TaxaJuros = taxaResult.Value,
                PrazoMeses = prazoResult.Value,
                ValorDesejado = valorResult.Value,
                DataReferencia = DateOnly.FromDateTime(DateTime.Today)
            };

            // Adicionar resultado
            var resultado = new ResultadoSimulacao
            {
                ValorTotal = valorResult.Value,
                Parcelas = new List<Parcela>()
            };

            // Adicionar parcelas com números únicos
            for (int j = 1; j <= 3; j++)
            {
                resultado.Parcelas.Add(new Parcela
                {
                    Numero = j, // Número único para cada parcela
                    ValorPrestacao = valorResult.Value,
                    ValorAmortizacao = valorResult.Value,
                    ValorJuros = ValorMonetario.Create(100m).Value
                });
            }

            simulacao.Resultados = new List<ResultadoSimulacao> { resultado };
            simulacoes.Add(simulacao);
        }

        return simulacoes;
    }
}