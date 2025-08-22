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
    public static void Configure()
    {
        // Request to Command/Query mappings
        TypeAdapterConfig<SimulacaoRequest, RealizarSimulacaoCommand>
            .NewConfig()
            .Map(dest => dest.Valor, src => src.ValorDesejado)
            .Map(dest => dest.Prazo, src => src.Prazo);

        TypeAdapterConfig<ListarSimulacoesRequest, ListarSimulacoesQuery>
            .NewConfig()
            .Map(dest => dest.NumeroPagina, src => src.Pagina)
            .Map(dest => dest.TamanhoPagina, src => src.QtdRegistrosPagina);

        // Result to Response mappings
        TypeAdapterConfig<SimulacaoResult, SimulacaoResponse>
            .NewConfig()
            .Map(dest => dest.IdSimulacao, src => src.Id)
            .Map(dest => dest.ResultadoSimulacao, src => src.Resultados);
        

        TypeAdapterConfig<ResultadoCalculoAmortizacao, ResultadoSimulacaoResponse>
            .NewConfig()
            .Map(dest => dest.Tipo, src => src.TipoAmortizacao)
            .Map(dest => dest.Parcelas, src => src.Parcelas);

        TypeAdapterConfig<ParcelaCalculada, ParcelaResponse>
            .NewConfig();

        // Paged Result mappings
        TypeAdapterConfig<PagedResult<SimulacaoResumoResult>, ListarSimulacoesResponse>
            .NewConfig()
            .Map(dest => dest.Pagina, src => src.CurrentPage)
            .Map(dest => dest.QtdRegistros, src => src.TotalItems)
            .Map(dest => dest.QtdRegistrosPagina, src => src.PageSize)
            .Map(dest => dest.Registros, src => src.Items);

        TypeAdapterConfig<SimulacaoResumoResult, SimulacaoResumoResponse>
            .NewConfig()
            .Map(dest => dest.IdSimulacao, src => src.Id);

        // Volume Result mappings
        TypeAdapterConfig<VolumeSimuladoResult, VolumeSimuladoResponse>
            .NewConfig()
            .Map(dest => dest.DataReferencia, src => src.DataReferencia.ToString("yyyy-MM-dd"))
            .Map(dest => dest.Simulacoes, src => src.Produtos);

        TypeAdapterConfig<VolumeSimuladoProdutoResult, VolumeSimuladoProdutoResponse>
            .NewConfig();
    }
}
