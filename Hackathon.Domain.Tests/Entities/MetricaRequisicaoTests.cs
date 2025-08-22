using FluentAssertions;
using Hackathon.Domain.Entities;
using Xunit;

namespace Hackathon.Domain.Tests.Entities;

public class MetricaRequisicaoTests
{
    [Fact]
    public void Construtor_DeveCriarMetricaComValoresPadrao()
    {
        // Act
        var metrica = new MetricaRequisicao();

        // Assert
        metrica.Id.Should().Be(0);
        metrica.NomeApi.Should().Be(string.Empty);
        metrica.Endpoint.Should().Be(string.Empty);
        metrica.TempoRespostaMs.Should().Be(0);
        metrica.Sucesso.Should().BeFalse();
        metrica.StatusCode.Should().Be(0);
        metrica.DataHora.Should().Be(default(DateTime));
    }

    [Fact]
    public void Propriedades_DevePermitirDefinirEObterValores()
    {
        // Arrange
        var metrica = new MetricaRequisicao();
        var dataHora = DateTime.Now;
        var nomeApi = "Simulacao";
        var endpoint = "POST /simulacao";
        var tempoResposta = 150L;
        var sucesso = true;
        var statusCode = 200;

        // Act
        metrica.Id = 1;
        metrica.NomeApi = nomeApi;
        metrica.Endpoint = endpoint;
        metrica.TempoRespostaMs = tempoResposta;
        metrica.Sucesso = sucesso;
        metrica.StatusCode = statusCode;
        metrica.DataHora = dataHora;

        // Assert
        metrica.Id.Should().Be(1);
        metrica.NomeApi.Should().Be(nomeApi);
        metrica.Endpoint.Should().Be(endpoint);
        metrica.TempoRespostaMs.Should().Be(tempoResposta);
        metrica.Sucesso.Should().Be(sucesso);
        metrica.StatusCode.Should().Be(statusCode);
        metrica.DataHora.Should().Be(dataHora);
    }

    [Fact]
    public void Propriedades_DevePermitirDefinirValoresNulos()
    {
        // Arrange
        var metrica = new MetricaRequisicao();

        // Act
        metrica.NomeApi = null!;
        metrica.Endpoint = null!;

        // Assert
        metrica.NomeApi.Should().BeNull();
        metrica.Endpoint.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(-1)]
    public void TempoRespostaMs_DeveAceitarDiferentesValores(long tempoResposta)
    {
        // Arrange
        var metrica = new MetricaRequisicao();

        // Act
        metrica.TempoRespostaMs = tempoResposta;

        // Assert
        metrica.TempoRespostaMs.Should().Be(tempoResposta);
    }

    [Theory]
    [InlineData(200, true)]
    [InlineData(201, true)]
    [InlineData(299, true)]
    [InlineData(400, false)]
    [InlineData(404, false)]
    [InlineData(500, false)]
    public void StatusCode_DevePermitirDiferentesCodigos(int statusCode, bool sucessoEsperado)
    {
        // Arrange
        var metrica = new MetricaRequisicao();

        // Act
        metrica.StatusCode = statusCode;
        metrica.Sucesso = sucessoEsperado;

        // Assert
        metrica.StatusCode.Should().Be(statusCode);
        metrica.Sucesso.Should().Be(sucessoEsperado);
    }

    [Fact]
    public void DataHora_DevePermitirDefinirDataHoraEspecifica()
    {
        // Arrange
        var metrica = new MetricaRequisicao();
        var dataHora = new DateTime(2024, 1, 15, 10, 30, 45);

        // Act
        metrica.DataHora = dataHora;

        // Assert
        metrica.DataHora.Should().Be(dataHora);
        metrica.DataHora.Year.Should().Be(2024);
        metrica.DataHora.Month.Should().Be(1);
        metrica.DataHora.Day.Should().Be(15);
        metrica.DataHora.Hour.Should().Be(10);
        metrica.DataHora.Minute.Should().Be(30);
        metrica.DataHora.Second.Should().Be(45);
    }

    [Fact]
    public void MetricaRequisicao_DeveSerClassePublica()
    {
        // Act & Assert
        typeof(MetricaRequisicao).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void MetricaRequisicao_DeveTerPropriedadesPublicas()
    {
        // Arrange
        var tipo = typeof(MetricaRequisicao);

        // Act & Assert
        tipo.GetProperty("Id")!.Should().NotBeNull();
        tipo.GetProperty("NomeApi")!.Should().NotBeNull();
        tipo.GetProperty("Endpoint")!.Should().NotBeNull();
        tipo.GetProperty("TempoRespostaMs")!.Should().NotBeNull();
        tipo.GetProperty("Sucesso")!.Should().NotBeNull();
        tipo.GetProperty("StatusCode")!.Should().NotBeNull();
        tipo.GetProperty("DataHora")!.Should().NotBeNull();
    }
}
