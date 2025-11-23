using Mapster;
using Hackathon.API.Contracts.Requests;
using Hackathon.API.Contracts.Responses;
using Hackathon.Application.Commands;
using Hackathon.Application.Queries;
using Hackathon.Application.Results;

namespace Hackathon.API.Mappings;

/// <summary>
/// Profile de mapeamento para conversões entre API Contracts e Application Commands/Queries/Results
/// </summary>
public static class ApiMappingProfile
{
    public static void Configure(TypeAdapterConfig? config = null)
    {
        var cfg = config ?? TypeAdapterConfig.GlobalSettings;

        cfg.NewConfig<SimulacaoRequest, RealizarSimulacaoCommand>()
            .Map(dest => dest.Valor, src => src.ValorDesejado)
            .Map(dest => dest.Prazo, src => src.Prazo);

        cfg.NewConfig<ListarSimulacoesRequest, ListarSimulacoesQuery>()
            .Map(dest => dest.NumeroPagina, src => src.Pagina)
            .Map(dest => dest.TamanhoPagina, src => src.QtdRegistrosPagina);

        cfg.NewConfig<SimulacaoResult, SimulacaoResponse>()
            .Map(dest => dest.IdSimulacao, src => src.Id)
            .Map(dest => dest.ResultadoSimulacao, src => src.Resultados);

        cfg.NewConfig<ResultadoCalculoAmortizacao, ResultadoSimulacaoResponse>()
            .Map(dest => dest.Tipo, src => src.TipoAmortizacao)
            .Map(dest => dest.Parcelas, src => src.Parcelas);

        cfg.NewConfig<ParcelaCalculada, ParcelaResponse>();

        cfg.NewConfig<PagedResult<SimulacaoResumoResult>, ListarSimulacoesResponse>()
            .Map(dest => dest.Pagina, src => src.CurrentPage)
            .Map(dest => dest.QtdRegistros, src => src.TotalItems)
            .Map(dest => dest.QtdRegistrosPagina, src => src.PageSize)
            .Map(dest => dest.Registros, src => src.Items);

        cfg.NewConfig<SimulacaoResumoResult, SimulacaoResumoResponse>()
            .Map(dest => dest.IdSimulacao, src => src.Id);

        cfg.NewConfig<ValorTotalAmortizacaoResult, ValorTotalAmortizacaoResponse>();

        cfg.NewConfig<VolumeSimuladoResult, VolumeSimuladoResponse>()
            .Map(dest => dest.DataReferencia, src => src.DataReferencia.ToString("yyyy-MM-dd"))
            .Map(dest => dest.Simulacoes, src => src.Produtos);

        cfg.NewConfig<VolumeSimuladoProdutoResult, VolumeSimuladoProdutoResponse>();
    }
}
