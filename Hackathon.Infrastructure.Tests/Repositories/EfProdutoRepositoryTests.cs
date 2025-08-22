using Microsoft.EntityFrameworkCore;

namespace Hackathon.Infrastructure.Tests.Repositories;

public class EfProdutoRepositoryTests
{
    [Fact]
    public void Constructor_ComParametrosValidos_DeveCriarInstancia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProdutoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        using var context = new ProdutoDbContext(options);
        var logger = new Mock<ILogger<EfProdutoRepository>>();

        // Act & Assert
        FluentActions.Invoking(() => new EfProdutoRepository(context, logger.Object))
            .Should().NotThrow();
    }

    [Fact]
    public async Task GetAllAsync_ComDados_DeveRetornarProdutos()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProdutoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ProdutoDbContext(options);
        var logger = new Mock<ILogger<EfProdutoRepository>>();
        var repository = new EfProdutoRepository(context, logger.Object);

        // Adicionar dados de teste
        var produtos = CreateListaProdutos();
        context.Produtos.AddRange(produtos);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(produtos.Count);
    }

    [Fact]
    public async Task GetAllAsync_SemDados_DeveRetornarListaVazia()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProdutoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ProdutoDbContext(options);
        var logger = new Mock<ILogger<EfProdutoRepository>>();
        var repository = new EfProdutoRepository(context, logger.Object);

        // Act
        var resultado = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ComExcecao_DeveLogarErroERetornarNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProdutoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ProdutoDbContext(options);
        var logger = new Mock<ILogger<EfProdutoRepository>>();
        var repository = new EfProdutoRepository(context, logger.Object);

        // Dispose do contexto para forçar erro
        context.Dispose();

        // Act
        var resultado = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
        
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

    private static List<Produto> CreateListaProdutos()
    {
        var taxa1 = TaxaJuros.Create(0.015m).Value;
        var taxa2 = TaxaJuros.Create(0.020m).Value;
        var taxa3 = TaxaJuros.Create(0.025m).Value;
        
        var valorMin1 = ValorMonetario.Create(1000m).Value;
        var valorMax1 = ValorMonetario.Create(50000m).Value;
        var valorMin2 = ValorMonetario.Create(5000m).Value;
        var valorMax2 = ValorMonetario.Create(100000m).Value;
        var valorMin3 = ValorMonetario.Create(10000m).Value;
        var valorMax3 = ValorMonetario.Create(200000m).Value;
        
        return new List<Produto>
        {
            new() { Codigo = 1, Descricao = "Crédito Pessoal", TaxaMensal = taxa1, MinMeses = 12, MaxMeses = 60, MinValor = valorMin1, MaxValor = valorMax1 },
            new() { Codigo = 2, Descricao = "Crédito Consignado", TaxaMensal = taxa2, MinMeses = 6, MaxMeses = 84, MinValor = valorMin2, MaxValor = valorMax2 },
            new() { Codigo = 3, Descricao = "Crédito Imobiliário", TaxaMensal = taxa3, MinMeses = 24, MaxMeses = 420, MinValor = valorMin3, MaxValor = valorMax3 }
        };
    }
}