using Hackathon.Application.Queries;
using Hackathon.Application.Results;
using Hackathon.Domain.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace Hackathon.Application.Handlers;

/// <summary>
/// Handler para obter volume simulado
/// SRP: Apenas coordena consulta de dados agregados
/// </summary>
public class ObterVolumeSimuladoHandler : IRequestHandler<ObterVolumeSimuladoQuery, VolumeSimuladoResult>
{
    private readonly ISimulacaoRepository _repository;

    public ObterVolumeSimuladoHandler(ISimulacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task<VolumeSimuladoResult> Handle(ObterVolumeSimuladoQuery request, CancellationToken cancellationToken)
    {
        var dadosAgregados = await _repository.ObterVolumeSimuladoPorProdutoAsync(request.DataReferencia, cancellationToken);
        
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