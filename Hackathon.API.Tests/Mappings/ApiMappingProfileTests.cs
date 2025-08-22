using FluentAssertions;
using Hackathon.API.Contracts.Requests;
using Hackathon.API.Contracts.Responses;
using Hackathon.API.Mappings;
using Hackathon.Application.Commands;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Mapster;

namespace Hackathon.API.Tests.Mappings;

public class ApiMappingProfileTests
{
    public ApiMappingProfileTests()
    {
        // Configurar o Mapster antes de cada teste
        ApiMappingProfile.Configure();
        TypeAdapterConfig.GlobalSettings.Compile();
    }

    [Fact]
    public void Configure_DeveConfigurarMapeamentosSemErro()
    {
        // Act & Assert
        var action = () => ApiMappingProfile.Configure();
        action.Should().NotThrow();
    }

    [Fact]
    public void SimulacaoRequestParaRealizarSimulacaoCommand_DeveMapearCorretamente()
    {
        // Arrange
        var request = new SimulacaoRequest(10000, 12);

        // Act
        var command = request.Adapt<RealizarSimulacaoCommand>();

        // Assert
        command.Should().NotBeNull();
        command.Valor.Should().Be(10000);
        command.Prazo.Should().Be(12);
    }

    [Fact]
    public void ListarSimulacoesRequestParaListarSimulacoesQuery_DeveMapearCorretamente()
    {
        // Arrange
        var request = new ListarSimulacoesRequest(2, 20);

        // Act
        var query = request.Adapt<ListarSimulacoesQuery>();

        // Assert
        query.Should().NotBeNull();
        query.NumeroPagina.Should().Be(2);
        query.TamanhoPagina.Should().Be(20);
    }

    [Fact]
    public void SimulacaoResultParaSimulacaoResponse_DeveMapearCorretamente()
    {
        // Arrange
        var simulacaoId = Guid.NewGuid();
        var result = new SimulacaoResult(
            simulacaoId,
            1,
            "Produto Teste",
            0.05m,
            new List<ResultadoCalculoAmortizacao>
            {
                new("SAC", new List<ParcelaCalculada>())
            }
        );

        // Act
        var response = result.Adapt<SimulacaoResponse>();

        // Assert
        response.Should().NotBeNull();
        response.IdSimulacao.Should().Be(simulacaoId);
        response.ResultadoSimulacao.Should().HaveCount(1);
    }

    [Fact]
    public void ResultadoCalculoAmortizacaoParaResultadoSimulacaoResponse_DeveMapearCorretamente()
    {
        // Arrange
        var resultado = new ResultadoCalculoAmortizacao(
            "PRICE",
            new List<ParcelaCalculada>
            {
                new(1, 800m, 200m, 1000m)
            }
        );

        // Act
        var response = resultado.Adapt<ResultadoSimulacaoResponse>();

        // Assert
        response.Should().NotBeNull();
        response.Tipo.Should().Be("PRICE");
        response.Parcelas.Should().HaveCount(1);
    }

    [Fact]
    public void ParcelaCalculadaParaParcelaResponse_DeveMapearCorretamente()
    {
        // Arrange
        var parcela = new ParcelaCalculada(1, 800m, 200m, 1000m);

        // Act
        var response = parcela.Adapt<ParcelaResponse>();

        // Assert
        response.Should().NotBeNull();
        response.Numero.Should().Be(1);
        response.ValorAmortizacao.Should().Be(800m);
        response.ValorJuros.Should().Be(200m);
        response.ValorPrestacao.Should().Be(1000m);
    }

    [Fact]
    public void PagedResultParaListarSimulacoesResponse_DeveMapearCorretamente()
    {
        // Arrange
        var pagedResult = new PagedResult<SimulacaoResumoResult>(
            new List<SimulacaoResumoResult>
            {
                new(Guid.NewGuid(), 10000, 12, 12000)
            },
            25,
            2,
            10
        );

        // Act
        var response = pagedResult.Adapt<ListarSimulacoesResponse>();

        // Assert
        response.Should().NotBeNull();
        response.Pagina.Should().Be(2);
        response.QtdRegistros.Should().Be(25);
        response.QtdRegistrosPagina.Should().Be(10);
        response.Registros.Should().HaveCount(1);
    }

    [Fact]
    public void SimulacaoResumoResultParaSimulacaoResumoResponse_DeveMapearCorretamente()
    {
        // Arrange
        var simulacaoId = Guid.NewGuid();
        var resumo = new SimulacaoResumoResult(simulacaoId, 15000, 24, 18000);

        // Act
        var response = resumo.Adapt<SimulacaoResumoResponse>();

        // Assert
        response.Should().NotBeNull();
        response.IdSimulacao.Should().Be(simulacaoId);
        response.ValorDesejado.Should().Be(15000);
        response.Prazo.Should().Be(24);
        response.ValorTotalParcelas.Should().Be(18000);
    }

    [Fact]
    public void VolumeSimuladoResultParaVolumeSimuladoResponse_DeveMapearCorretamente()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.Today);
        var volumeResult = new VolumeSimuladoResult(
            dataReferencia,
            new List<VolumeSimuladoProdutoResult>
            {
                new(1, "Produto 1", 0.05m, 1000m, 10000m, 12000m)
            }
        );

        // Act
        var response = volumeResult.Adapt<VolumeSimuladoResponse>();

        // Assert
        response.Should().NotBeNull();
        response.DataReferencia.Should().Be(dataReferencia.ToString("yyyy-MM-dd"));
        response.Simulacoes.Should().HaveCount(1);
    }

    [Fact]
    public void VolumeSimuladoProdutoResultParaVolumeSimuladoProdutoResponse_DeveMapearCorretamente()
    {
        // Arrange
        var produto = new VolumeSimuladoProdutoResult(1, "Produto Teste", 0.05m, 1000m, 15000m, 18000m);

        // Act
        var response = produto.Adapt<VolumeSimuladoProdutoResponse>();

        // Assert
        response.Should().NotBeNull();
        response.CodigoProduto.Should().Be(1);
        response.DescricaoProduto.Should().Be("Produto Teste");
        response.TaxaMediaJuro.Should().Be(0.05m);
        response.ValorMedioPrestacao.Should().Be(1000m);
        response.ValorTotalDesejado.Should().Be(15000m);
        response.ValorTotalCredito.Should().Be(18000m);
    }

    [Fact]
    public void MapeamentosComValoresNulos_DeveTratarCorretamente()
    {
        // Arrange
        var simulacaoRequest = new SimulacaoRequest(0, 0);

        // Act
        var command = simulacaoRequest.Adapt<RealizarSimulacaoCommand>();

        // Assert
        command.Should().NotBeNull();
        command.Valor.Should().Be(0);
        command.Prazo.Should().Be(0);
    }

    [Fact]
    public void MapeamentosComListasVazias_DeveTratarCorretamente()
    {
        // Arrange
        var simulacaoResult = new SimulacaoResult(
            Guid.NewGuid(),
            1,
            "Produto Teste",
            0.05m,
            new List<ResultadoCalculoAmortizacao>()
        );

        // Act
        var response = simulacaoResult.Adapt<SimulacaoResponse>();

        // Assert
        response.Should().NotBeNull();
        response.ResultadoSimulacao.Should().NotBeNull();
        response.ResultadoSimulacao.Should().BeEmpty();
    }

    [Fact]
    public void MapeamentosComPagedResultVazio_DeveTratarCorretamente()
    {
        // Arrange
        var pagedResult = new PagedResult<SimulacaoResumoResult>(
            new List<SimulacaoResumoResult>(),
            0,
            1,
            10
        );

        // Act
        var response = pagedResult.Adapt<ListarSimulacoesResponse>();

        // Assert
        response.Should().NotBeNull();
        response.Pagina.Should().Be(1);
        response.QtdRegistros.Should().Be(0);
        response.QtdRegistrosPagina.Should().Be(10);
        response.Registros.Should().NotBeNull();
        response.Registros.Should().BeEmpty();
    }
}
