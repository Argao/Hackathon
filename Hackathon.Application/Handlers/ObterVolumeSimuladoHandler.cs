using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Application.Services;
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