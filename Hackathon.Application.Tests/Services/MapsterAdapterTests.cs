using Hackathon.Application.Services;

namespace Hackathon.Application.Tests.Services;

public class MapsterAdapterTests
{
    private readonly MapsterAdapter _adapter;

    public MapsterAdapterTests()
    {
        _adapter = new MapsterAdapter();
    }

    [Fact]
    public void Map_ComObjetoValido_DeveMapearCorretamente()
    {
        // Arrange
        var source = new TestSource { Id = 1, Nome = "Teste", Valor = 100.50m };

        // Act
        var result = _adapter.Map<TestSource, TestDestination>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Nome, result.Nome);
        Assert.Equal(source.Valor, result.Valor);
    }

    [Fact]
    public void Map_ComObjetoNull_DeveRetornarDefault()
    {
        // Arrange
        TestSource source = null;

        // Act
        var result = _adapter.Map<TestSource, TestDestination>(source);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void MapCollection_ComColecaoValida_DeveMapearTodosOsItens()
    {
        // Arrange
        var sources = new List<TestSource>
        {
            new() { Id = 1, Nome = "Item 1", Valor = 100m },
            new() { Id = 2, Nome = "Item 2", Valor = 200m },
            new() { Id = 3, Nome = "Item 3", Valor = 300m }
        };

        // Act
        var results = _adapter.MapCollection<TestSource, TestDestination>(sources).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(3, results.Count);
        Assert.Equal(1, results[0].Id);
        Assert.Equal("Item 1", results[0].Nome);
        Assert.Equal(100m, results[0].Valor);
        Assert.Equal(2, results[1].Id);
        Assert.Equal("Item 2", results[1].Nome);
        Assert.Equal(200m, results[1].Valor);
        Assert.Equal(3, results[2].Id);
        Assert.Equal("Item 3", results[2].Nome);
        Assert.Equal(300m, results[2].Valor);
    }

    [Fact]
    public void MapCollection_ComColecaoNull_DeveRetornarColecaoVazia()
    {
        // Arrange
        IEnumerable<TestSource> sources = null;

        // Act
        var results = _adapter.MapCollection<TestSource, TestDestination>(sources);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public void MapCollection_ComColecaoVazia_DeveRetornarColecaoVazia()
    {
        // Arrange
        var sources = new List<TestSource>();

        // Act
        var results = _adapter.MapCollection<TestSource, TestDestination>(sources);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public void Map_ComConfigureAction_DeveAplicarConfiguracao()
    {
        // Arrange
        var source = new TestSource { Id = 1, Nome = "Teste", Valor = 100m };
        var configuracaoAplicada = false;

        // Act
        var result = _adapter.Map<TestSource, TestDestination>(source, (src, dest) =>
        {
            dest.Nome = $"{src.Nome} - Configurado";
            configuracaoAplicada = true;
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Teste - Configurado", result.Nome);
        Assert.Equal(100m, result.Valor);
        Assert.True(configuracaoAplicada);
    }

    [Fact]
    public void Map_ComConfigureActionNull_DeveMapearSemConfiguracao()
    {
        // Arrange
        var source = new TestSource { Id = 1, Nome = "Teste", Valor = 100m };

        // Act
        var result = _adapter.Map<TestSource, TestDestination>(source, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Nome, result.Nome);
        Assert.Equal(source.Valor, result.Valor);
    }

    [Fact]
    public void Map_ComObjetoComplexo_DeveMapearPropriedadesAninhadas()
    {
        // Arrange
        var source = new ComplexSource
        {
            Id = 1,
            Dados = new TestSource { Id = 2, Nome = "Aninhado", Valor = 50m }
        };

        // Act
        var result = _adapter.Map<ComplexSource, ComplexDestination>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.Id, result.Id);
        Assert.NotNull(result.Dados);
        Assert.Equal(source.Dados.Id, result.Dados.Id);
        Assert.Equal(source.Dados.Nome, result.Dados.Nome);
        Assert.Equal(source.Dados.Valor, result.Dados.Valor);
    }

    // Classes de teste para mapeamento
    public class TestSource
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
    }

    public class TestDestination
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
    }

    public class ComplexSource
    {
        public int Id { get; set; }
        public TestSource Dados { get; set; }
    }

    public class ComplexDestination
    {
        public int Id { get; set; }
        public TestDestination Dados { get; set; }
    }
}
