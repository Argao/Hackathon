using FluentAssertions;
using Hackathon.Domain.Interfaces.Services;
using Xunit;

namespace Hackathon.Domain.Tests.Interfaces.Services;

public class IEventHubServiceTests
{
    [Fact]
    public void IEventHubService_DeveSerInterface()
    {
        // Act & Assert
        typeof(IEventHubService).IsInterface.Should().BeTrue();
    }

    [Fact]
    public void IEventHubService_DeveSerPublica()
    {
        // Act & Assert
        typeof(IEventHubService).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void IEventHubService_DeveTerMetodoEnviarSimulacaoAsyncComString()
    {
        // Arrange
        var tipo = typeof(IEventHubService);

        // Act
        var metodo = tipo.GetMethod("EnviarSimulacaoAsync", new[] { typeof(string), typeof(CancellationToken) });

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void IEventHubService_DeveTerMetodoEnviarSimulacaoAsyncComGenerico()
    {
        // Arrange
        var tipo = typeof(IEventHubService);

        // Act
        var metodos = tipo.GetMethods().Where(m => m.Name == "EnviarSimulacaoAsync" && m.IsGenericMethod).ToList();

        // Assert
        metodos.Should().NotBeEmpty();
        var metodo = metodos.First();
        metodo.ReturnType.Should().Be(typeof(Task));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
        metodo.IsGenericMethod.Should().BeTrue();
    }

    [Fact]
    public void IEventHubService_DeveTerMetodoEnviarSimulacaoComString()
    {
        // Arrange
        var tipo = typeof(IEventHubService);

        // Act
        var metodo = tipo.GetMethod("EnviarSimulacao", new[] { typeof(string) });

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(void));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void IEventHubService_DeveTerMetodoEnviarSimulacaoComGenerico()
    {
        // Arrange
        var tipo = typeof(IEventHubService);

        // Act
        var metodos = tipo.GetMethods().Where(m => m.Name == "EnviarSimulacao" && m.IsGenericMethod).ToList();

        // Assert
        metodos.Should().NotBeEmpty();
        var metodo = metodos.First();
        metodo.ReturnType.Should().Be(typeof(void));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
        metodo.IsGenericMethod.Should().BeTrue();
    }

    [Fact]
    public void IEventHubService_DeveTerParametrosCorretosNoMetodoEnviarSimulacaoAsyncComString()
    {
        // Arrange
        var tipo = typeof(IEventHubService);
        var metodo = tipo.GetMethod("EnviarSimulacaoAsync", new[] { typeof(string), typeof(CancellationToken) });

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(2);
        parametros[0].ParameterType.Should().Be(typeof(string));
        parametros[0].Name.Should().Be("simulacaoData");
        parametros[1].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[1].Name.Should().Be("cancellationToken");
        parametros[1].HasDefaultValue.Should().BeTrue();
    }

    [Fact]
    public void IEventHubService_DeveTerParametrosCorretosNoMetodoEnviarSimulacaoAsyncComGenerico()
    {
        // Arrange
        var tipo = typeof(IEventHubService);
        var metodos = tipo.GetMethods().Where(m => m.Name == "EnviarSimulacaoAsync" && m.IsGenericMethod).ToList();

        // Act
        var metodo = metodos.First();
        var parametros = metodo.GetParameters();

        // Assert
        parametros.Should().HaveCount(2);
        parametros[0].ParameterType.Should().NotBeNull(); // T where T : class
        parametros[0].Name.Should().Be("simulacao");
        parametros[1].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[1].Name.Should().Be("cancellationToken");
        parametros[1].HasDefaultValue.Should().BeTrue();
    }

    [Fact]
    public void IEventHubService_DeveTerParametrosCorretosNoMetodoEnviarSimulacaoComString()
    {
        // Arrange
        var tipo = typeof(IEventHubService);
        var metodo = tipo.GetMethod("EnviarSimulacao", new[] { typeof(string) });

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(1);
        parametros[0].ParameterType.Should().Be(typeof(string));
        parametros[0].Name.Should().Be("simulacaoData");
    }

    [Fact]
    public void IEventHubService_DeveTerParametrosCorretosNoMetodoEnviarSimulacaoComGenerico()
    {
        // Arrange
        var tipo = typeof(IEventHubService);
        var metodos = tipo.GetMethods().Where(m => m.Name == "EnviarSimulacao" && m.IsGenericMethod).ToList();

        // Act
        var metodo = metodos.First();
        var parametros = metodo.GetParameters();

        // Assert
        parametros.Should().HaveCount(1);
        parametros[0].ParameterType.Should().NotBeNull();
        parametros[0].Name.Should().Be("simulacao");
    }

    [Fact]
    public void IEventHubService_DeveTerNamespaceCorreto()
    {
        // Act & Assert
        typeof(IEventHubService).Namespace.Should().Be("Hackathon.Domain.Interfaces.Services");
    }

    [Fact]
    public void IEventHubService_DeveTerNomeCorreto()
    {
        // Act & Assert
        typeof(IEventHubService).Name.Should().Be("IEventHubService");
    }

    [Fact]
    public void IEventHubService_DeveSerImplementavel()
    {
        // Arrange
        var tipo = typeof(IEventHubService);

        // Act & Assert
        tipo.IsAbstract.Should().BeTrue();
        tipo.IsSealed.Should().BeFalse();
    }

    [Fact]
    public void IEventHubService_DeveTerQuatroMetodos()
    {
        // Arrange
        var tipo = typeof(IEventHubService);

        // Act
        var metodos = tipo.GetMethods();

        // Assert
        metodos.Should().HaveCount(4);
    }
}
