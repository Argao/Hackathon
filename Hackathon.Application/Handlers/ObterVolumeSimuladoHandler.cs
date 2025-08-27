using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Application.Services;
using Hackathon.Domain.Exceptions;
using Mapster;
using MediatR;

namespace Hackathon.Application.Handlers;

/// <summary>
/// Handler para obter volume simulado com cache otimizado
/// SRP: Apenas coordena consulta de dados agregados com cache
/// </summary>
public class ObterVolumeSimuladoHandler : IRequestHandler<ObterVolumeSimuladoQuery, VolumeSimuladoResult>
{
    private readonly IVolumeSimuladoCacheService _cacheService;

    public ObterVolumeSimuladoHandler(IVolumeSimuladoCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<VolumeSimuladoResult> Handle(ObterVolumeSimuladoQuery request, CancellationToken cancellationToken)
    {
        // ✅ OTIMIZAÇÃO: Usar cache com estratégia híbrida
        var dadosAgregados = await _cacheService.GetVolumeSimuladoAsync(request.DataReferencia, cancellationToken);
        
        if (!dadosAgregados.Any())
        {
            throw new SimulacaoException($"Nenhum dado de volume simulado encontrado para a data {request.DataReferencia:yyyy-MM-dd}");
        }
        
        var produtos = dadosAgregados.Select(dto => new VolumeSimuladoProdutoResult(
            dto.CodigoProduto,
            dto.DescricaoProduto,
            dto.TaxaMediaJuro,
            dto.ValorMedioPrestacao,
            dto.ValorTotalDesejado,
            dto.ValorTotalCredito
        )).ToList();

        return new VolumeSimuladoResult(
            DataReferencia: request.DataReferencia,
            Produtos: produtos
        );
    }
}