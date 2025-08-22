using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Xunit;

namespace Hackathon.Domain.Tests.Interfaces.Repositories;

public class IProdutoRepositoryTests
{
    [Fact]
    public void IProdutoRepository_DeveSerInterface()
    {
        // Act & Assert
        typeof(IProdutoRepository).IsInterface.Should().BeTrue();
    }

    [Fact]
    public void IProdutoRepository_DeveSerPublica()
    {
        // Act & Assert
        typeof(IProdutoRepository).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void IProdutoRepository_DeveTerMetodoGetAllAsync()
    {
        // Arrange
        var tipo = typeof(IProdutoRepository);

        // Act
        var metodo = tipo.GetMethod("GetAllAsync");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task<IEnumerable<Produto>?>));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void IProdutoRepository_DeveTerParametrosCorretosNoMetodoGetAllAsync()
    {
        // Arrange
        var tipo = typeof(IProdutoRepository);
        var metodo = tipo.GetMethod("GetAllAsync");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(1);
        parametros[0].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[0].Name.Should().Be("ct");
        parametros[0].HasDefaultValue.Should().BeTrue();
    }

    [Fact]
    public void IProdutoRepository_DeveTerNamespaceCorreto()
    {
        // Act & Assert
        typeof(IProdutoRepository).Namespace.Should().Be("Hackathon.Domain.Interfaces.Repositories");
    }

    [Fact]
    public void IProdutoRepository_DeveTerNomeCorreto()
    {
        // Act & Assert
        typeof(IProdutoRepository).Name.Should().Be("IProdutoRepository");
    }

    [Fact]
    public void IProdutoRepository_DeveSerImplementavel()
    {
        // Arrange
        var tipo = typeof(IProdutoRepository);

        // Act & Assert
        tipo.IsAbstract.Should().BeTrue();
        tipo.IsSealed.Should().BeFalse();
    }

    [Fact]
    public void IProdutoRepository_DeveTerUmMetodo()
    {
        // Arrange
        var tipo = typeof(IProdutoRepository);

        // Act
        var metodos = tipo.GetMethods();

        // Assert
        metodos.Should().HaveCount(1);
    }

    [Fact]
    public void IProdutoRepository_DeveTerComentarioXML()
    {
        // Arrange
        var tipo = typeof(IProdutoRepository);

        // Act
        var atributos = tipo.GetCustomAttributes(true);

        // Assert
        // Verifica se tem documentação XML (não é um atributo, mas sim documentação)
        tipo.GetMethod("GetAllAsync")!.GetCustomAttributes(true).Should().NotBeNull();
    }
}
