using FluentAssertions;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Xunit;

namespace Hackathon.Domain.Tests.Interfaces.Repositories;

public class IMetricaRepositoryTests
{
    [Fact]
    public void IMetricaRepository_DeveSerInterface()
    {
        // Act & Assert
        typeof(IMetricaRepository).IsInterface.Should().BeTrue();
    }

    [Fact]
    public void IMetricaRepository_DeveSerPublica()
    {
        // Act & Assert
        typeof(IMetricaRepository).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void IMetricaRepository_DeveTerMetodoSalvarMetricaAsync()
    {
        // Arrange
        var tipo = typeof(IMetricaRepository);

        // Act
        var metodo = tipo.GetMethod("SalvarMetricaAsync");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void IMetricaRepository_DeveTerMetodoObterMetricasPorDataAsync()
    {
        // Arrange
        var tipo = typeof(IMetricaRepository);

        // Act
        var metodo = tipo.GetMethod("ObterMetricasPorDataAsync");

        // Assert
        metodo.Should().NotBeNull();
        metodo!.ReturnType.Should().Be(typeof(Task<List<MetricaAgregada>>));
        metodo.IsPublic.Should().BeTrue();
        metodo.IsVirtual.Should().BeTrue();
        metodo.IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void IMetricaRepository_DeveTerParametrosCorretosNoMetodoSalvarMetricaAsync()
    {
        // Arrange
        var tipo = typeof(IMetricaRepository);
        var metodo = tipo.GetMethod("SalvarMetricaAsync");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(2);
        parametros[0].ParameterType.Should().Be(typeof(MetricaRequisicao));
        parametros[0].Name.Should().Be("metrica");
        parametros[1].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[1].Name.Should().Be("cancellationToken");
        parametros[1].HasDefaultValue.Should().BeTrue();
    }

    [Fact]
    public void IMetricaRepository_DeveTerParametrosCorretosNoMetodoObterMetricasPorDataAsync()
    {
        // Arrange
        var tipo = typeof(IMetricaRepository);
        var metodo = tipo.GetMethod("ObterMetricasPorDataAsync");

        // Act
        var parametros = metodo!.GetParameters();

        // Assert
        parametros.Should().HaveCount(2);
        parametros[0].ParameterType.Should().Be(typeof(DateOnly));
        parametros[0].Name.Should().Be("dataReferencia");
        parametros[1].ParameterType.Should().Be(typeof(CancellationToken));
        parametros[1].Name.Should().Be("cancellationToken");
        parametros[1].HasDefaultValue.Should().BeTrue();
    }

    [Fact]
    public void IMetricaRepository_DeveTerNamespaceCorreto()
    {
        // Act & Assert
        typeof(IMetricaRepository).Namespace.Should().Be("Hackathon.Domain.Interfaces.Repositories");
    }

    [Fact]
    public void IMetricaRepository_DeveTerNomeCorreto()
    {
        // Act & Assert
        typeof(IMetricaRepository).Name.Should().Be("IMetricaRepository");
    }

    [Fact]
    public void IMetricaRepository_DeveSerImplementavel()
    {
        // Arrange
        var tipo = typeof(IMetricaRepository);

        // Act & Assert
        tipo.IsAbstract.Should().BeTrue();
        tipo.IsSealed.Should().BeFalse();
    }

    [Fact]
    public void IMetricaRepository_DeveTerDoisMetodos()
    {
        // Arrange
        var tipo = typeof(IMetricaRepository);

        // Act
        var metodos = tipo.GetMethods();

        // Assert
        metodos.Should().HaveCount(2);
    }
}

public class MetricaAgregadaTests
{
    [Fact]
    public void MetricaAgregada_DeveSerClassePublica()
    {
        // Act & Assert
        typeof(MetricaAgregada).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void MetricaAgregada_DeveSerClasseNaoSealed()
    {
        // Act & Assert
        typeof(MetricaAgregada).IsSealed.Should().BeFalse();
    }

    [Fact]
    public void MetricaAgregada_DeveTerPropriedadesCorretas()
    {
        // Arrange
        var tipo = typeof(MetricaAgregada);

        // Act & Assert
        tipo.GetProperty("NomeApi")!.PropertyType.Should().Be(typeof(string));
        tipo.GetProperty("Endpoint")!.PropertyType.Should().Be(typeof(string));
        tipo.GetProperty("QtdRequisicoes")!.PropertyType.Should().Be(typeof(int));
        tipo.GetProperty("TempoMedio")!.PropertyType.Should().Be(typeof(double));
        tipo.GetProperty("TempoMinimo")!.PropertyType.Should().Be(typeof(long));
        tipo.GetProperty("TempoMaximo")!.PropertyType.Should().Be(typeof(long));
        tipo.GetProperty("PercentualSucesso")!.PropertyType.Should().Be(typeof(double));
    }

    [Fact]
    public void MetricaAgregada_DeveTerPropriedadesPublicas()
    {
        // Arrange
        var tipo = typeof(MetricaAgregada);

        // Act & Assert
        tipo.GetProperty("NomeApi")!.Should().NotBeNull();
        tipo.GetProperty("Endpoint")!.Should().NotBeNull();
        tipo.GetProperty("QtdRequisicoes")!.Should().NotBeNull();
        tipo.GetProperty("TempoMedio")!.Should().NotBeNull();
        tipo.GetProperty("TempoMinimo")!.Should().NotBeNull();
        tipo.GetProperty("TempoMaximo")!.Should().NotBeNull();
        tipo.GetProperty("PercentualSucesso")!.Should().NotBeNull();
    }

    [Fact]
    public void MetricaAgregada_DeveTerPropriedadesComGetESet()
    {
        // Arrange
        var tipo = typeof(MetricaAgregada);

        // Act & Assert
        tipo.GetProperty("NomeApi")!.CanRead.Should().BeTrue();
        tipo.GetProperty("NomeApi")!.CanWrite.Should().BeTrue();
        tipo.GetProperty("Endpoint")!.CanRead.Should().BeTrue();
        tipo.GetProperty("Endpoint")!.CanWrite.Should().BeTrue();
        tipo.GetProperty("QtdRequisicoes")!.CanRead.Should().BeTrue();
        tipo.GetProperty("QtdRequisicoes")!.CanWrite.Should().BeTrue();
        tipo.GetProperty("TempoMedio")!.CanRead.Should().BeTrue();
        tipo.GetProperty("TempoMedio")!.CanWrite.Should().BeTrue();
        tipo.GetProperty("TempoMinimo")!.CanRead.Should().BeTrue();
        tipo.GetProperty("TempoMinimo")!.CanWrite.Should().BeTrue();
        tipo.GetProperty("TempoMaximo")!.CanRead.Should().BeTrue();
        tipo.GetProperty("TempoMaximo")!.CanWrite.Should().BeTrue();
        tipo.GetProperty("PercentualSucesso")!.CanRead.Should().BeTrue();
        tipo.GetProperty("PercentualSucesso")!.CanWrite.Should().BeTrue();
    }

    [Fact]
    public void MetricaAgregada_DeveTerValoresPadraoCorretos()
    {
        // Act
        var metrica = new MetricaAgregada();

        // Assert
        metrica.NomeApi.Should().Be(string.Empty);
        metrica.Endpoint.Should().Be(string.Empty);
        metrica.QtdRequisicoes.Should().Be(0);
        metrica.TempoMedio.Should().Be(0.0);
        metrica.TempoMinimo.Should().Be(0L);
        metrica.TempoMaximo.Should().Be(0L);
        metrica.PercentualSucesso.Should().Be(0.0);
    }

    [Fact]
    public void MetricaAgregada_DevePermitirDefinirEObterValores()
    {
        // Arrange
        var metrica = new MetricaAgregada();
        var nomeApi = "Simulacao";
        var endpoint = "POST /simulacao";
        var qtdRequisicoes = 100;
        var tempoMedio = 150.5;
        var tempoMinimo = 50L;
        var tempoMaximo = 500L;
        var percentualSucesso = 95.5;

        // Act
        metrica.NomeApi = nomeApi;
        metrica.Endpoint = endpoint;
        metrica.QtdRequisicoes = qtdRequisicoes;
        metrica.TempoMedio = tempoMedio;
        metrica.TempoMinimo = tempoMinimo;
        metrica.TempoMaximo = tempoMaximo;
        metrica.PercentualSucesso = percentualSucesso;

        // Assert
        metrica.NomeApi.Should().Be(nomeApi);
        metrica.Endpoint.Should().Be(endpoint);
        metrica.QtdRequisicoes.Should().Be(qtdRequisicoes);
        metrica.TempoMedio.Should().Be(tempoMedio);
        metrica.TempoMinimo.Should().Be(tempoMinimo);
        metrica.TempoMaximo.Should().Be(tempoMaximo);
        metrica.PercentualSucesso.Should().Be(percentualSucesso);
    }

    [Fact]
    public void MetricaAgregada_DeveTerNamespaceCorreto()
    {
        // Act & Assert
        typeof(MetricaAgregada).Namespace.Should().Be("Hackathon.Domain.Interfaces.Repositories");
    }

    [Fact]
    public void MetricaAgregada_DeveTerNomeCorreto()
    {
        // Act & Assert
        typeof(MetricaAgregada).Name.Should().Be("MetricaAgregada");
    }
}
