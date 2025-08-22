using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Xunit;

namespace Hackathon.Domain.Tests.Interfaces.Repositories;

public class ISimulacaoRepositoryTests
{
    [Fact]
    public void ISimulacaoRepository_DeveSerInterface()
    {
        // Act & Assert
        typeof(ISimulacaoRepository).IsInterface.Should().BeTrue();
    }

    [Fact]
    public void ISimulacaoRepository_DeveSerPublica()
    {
        // Act & Assert
        typeof(ISimulacaoRepository).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerMetodoAdicionarAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);

        // Act
        var metodo = tipo.GetMethod("AdicionarAsync");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task<Simulacao>));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerMetodoObterVolumeSimuladoPorProdutoAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);

        // Act
        var metodo = tipo.GetMethod("ObterVolumeSimuladoPorProdutoAsync");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task<IEnumerable<VolumeSimuladoAgregado>>));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerMetodoListarPaginadoAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);

        // Act
        var metodo = tipo.GetMethod("ListarPaginadoAsync");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task<(IEnumerable<Simulacao> Data, int TotalRecords)>));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerMetodoObterTotalSimulacoesAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);

        // Act
        var metodo = tipo.GetMethod("ObterTotalSimulacoesAsync");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task<int>));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerMetodoListarSimulacoesAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);

        // Act
        var metodo = tipo.GetMethod("ListarSimulacoesAsync");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task<IEnumerable<Simulacao>>));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerParametrosCorretosNoMetodoAdicionarAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);
        var metodo = tipo.GetMethod("AdicionarAsync");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(2);
        parametros[0].ParameterType.Should().Be(typeof(Simulacao));
        parametros[0].Name.Should().Be("simulacao");
        parametros[1].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[1].Name.Should().Be("ct");
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerParametrosCorretosNoMetodoObterVolumeSimuladoPorProdutoAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);
        var metodo = tipo.GetMethod("ObterVolumeSimuladoPorProdutoAsync");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(2);
        parametros[0].ParameterType.Should().Be(typeof(DateOnly));
        parametros[0].Name.Should().Be("dataReferencia");
        parametros[1].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[1].Name.Should().Be("ct");
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerParametrosCorretosNoMetodoListarPaginadoAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);
        var metodo = tipo.GetMethod("ListarPaginadoAsync");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(3);
        parametros[0].ParameterType.Should().Be(typeof(int));
        parametros[0].Name.Should().Be("pageNumber");
        parametros[1].ParameterType.Should().Be(typeof(int));
        parametros[1].Name.Should().Be("pageSize");
        parametros[2].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[2].Name.Should().Be("ct");
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerParametrosCorretosNoMetodoObterTotalSimulacoesAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);
        var metodo = tipo.GetMethod("ObterTotalSimulacoesAsync");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(1);
        parametros[0].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[0].Name.Should().Be("ct");
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerParametrosCorretosNoMetodoListarSimulacoesAsync()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);
        var metodo = tipo.GetMethod("ListarSimulacoesAsync");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(3);
        parametros[0].ParameterType.Should().Be(typeof(int));
        parametros[0].Name.Should().Be("pageNumber");
        parametros[1].ParameterType.Should().Be(typeof(int));
        parametros[1].Name.Should().Be("pageSize");
        parametros[2].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[2].Name.Should().Be("ct");
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerNamespaceCorreto()
    {
        // Act & Assert
        typeof(ISimulacaoRepository).Namespace.Should().Be("Hackathon.Domain.Interfaces.Repositories");
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerNomeCorreto()
    {
        // Act & Assert
        typeof(ISimulacaoRepository).Name.Should().Be("ISimulacaoRepository");
    }

    [Fact]
    public void ISimulacaoRepository_DeveSerImplementavel()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);

        // Act & Assert
        tipo.IsAbstract.Should().BeTrue();
        tipo.IsSealed.Should().BeFalse();
    }

    [Fact]
    public void ISimulacaoRepository_DeveTerCincoMetodos()
    {
        // Arrange
        var tipo = typeof(ISimulacaoRepository);

        // Act
        var metodos = tipo.GetMethods();

        // Assert
        metodos.Should().HaveCount(5);
    }
}
