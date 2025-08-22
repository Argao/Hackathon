using Mapster;
using Hackathon.Application.Commands;
using Hackathon.Application.Results;
using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.Mappings;

/// <summary>
/// Profile de mapeamento para conversões entre Application Commands/Results e Domain Entities
/// </summary>
public static class ApplicationMappingProfile
{
    public static void Configure()
    {
        // Command to Entity mappings
        TypeAdapterConfig<(RealizarSimulacaoCommand Command, Produto Produto), Simulacao>
            .NewConfig()
            .Map(dest => dest.CodigoProduto, src => src.Produto.Codigo)
            .Map(dest => dest.DescricaoProduto, src => src.Produto.Descricao)
            .Map(dest => dest.TaxaJuros, src => src.Produto.TaxaMensal)
            .Map(dest => dest.PrazoMeses, src => (short)src.Command.Prazo)
            .Map(dest => dest.ValorDesejado, src => ValorMonetario.Create(src.Command.Valor).Value)
            .Map(dest => dest.DataReferencia, src => DateOnly.FromDateTime(DateTime.Today))
            .Ignore(dest => dest.IdSimulacao)
            .Ignore(dest => dest.Resultados);

        // Entity to Result mappings
        TypeAdapterConfig<Simulacao, SimulacaoResult>
            .NewConfig()
            .Map(dest => dest.Id, src => src.IdSimulacao)
            .Map(dest => dest.Resultados, src => src.Resultados);

        TypeAdapterConfig<ResultadoSimulacao, ResultadoCalculoAmortizacao>
            .NewConfig()
            .Map(dest => dest.TipoAmortizacao, src => src.Tipo.ToString());

        TypeAdapterConfig<Parcela, ParcelaCalculada>
            .NewConfig();

        // Volume Entity mappings
        TypeAdapterConfig<VolumeSimuladoAgregado, VolumeSimuladoProdutoResult>
            .NewConfig();

        TypeAdapterConfig<Parcela, ParcelaCalculada>
            .NewConfig()
            .Map(dest => dest.ValorAmortizacao, src => src.ValorAmortizacao.Valor)
            .Map(dest => dest.ValorJuros, src => src.ValorJuros.Valor)
            .Map(dest => dest.ValorPrestacao, src => src.ValorPrestacao.Valor);
    }
}
