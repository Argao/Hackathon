using Hackathon.Application.DTOs.Requests;
using Hackathon.Application.DTOs.Responses;
using Hackathon.Application.Interfaces;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Domain.Interfaces.Services;

namespace Hackathon.Application.Services;

public class SimulacaoService : ISimulacaoService
{
    private readonly ISimulacaoRepository _simulacaoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IEnumerable<ICalculadoraAmortizacao> _calculadoras;

    public SimulacaoService(
        IProdutoRepository produtoRepository, 
        ISimulacaoRepository simulacaoRepository, 
        IEnumerable<ICalculadoraAmortizacao> calculadoras)
    {
        _produtoRepository = produtoRepository;
        _simulacaoRepository = simulacaoRepository;
        _calculadoras = calculadoras;
    }

    public async Task<SimulacaoResponseDTO?> RealizarSimulacaoAsync(SimulacaoRequestDTO request, CancellationToken ct)
    {
        var produto = await _produtoRepository.GetProdutoAdequadoAsync(request.Valor, request.Prazo, ct);
        if (produto is null) return null;
        
        var simulacao = CriarSimulacao(request, produto);
        
        var resultados = _calculadoras.Select(c => c.Calcular(request.Valor, produto.TaxaMensal, request.Prazo));
        simulacao.Resultados = resultados.ToList();
        
        await _simulacaoRepository.AdicionarAsync(simulacao, ct);

        return CriarResponseDto(simulacao);
    }
    
    public async Task<VolumeSimuladoResponseDTO> ObterVolumeSimuladoPorDiaAsync(DateOnly dataReferencia, CancellationToken ct)
    {
        var dadosAgregados = await _simulacaoRepository.ObterVolumeSimuladoPorProdutoAsync(dataReferencia, ct);
        
        var simulacoesDto = dadosAgregados.Select(d => new VolumeSimuladoProdutoDTO
        {
            CodigoProduto = d.CodigoProduto,
            DescricaoProduto = d.DescricaoProduto,
            TaxaMediaJuro = d.TaxaMediaJuro,
            ValorMedioPrestacao = d.ValorMedioPrestacao,
            ValorTotalDesejado = d.ValorTotalDesejado,
            ValorTotalCredito = d.ValorTotalCredito
        }).ToList();

        return new VolumeSimuladoResponseDTO
        {
            DataReferencia = dataReferencia.ToString("yyyy-MM-dd"),
            Simulacoes = simulacoesDto
        };
    }

    public async Task<PagedResponse<ListarSimulacoesDTO>> ListarPaginadoAsync(PagedRequest request, CancellationToken ct)
    {
        var (data, totalRecords) = await _simulacaoRepository.ListarPaginadoAsync(
            request.GetValidPageNumber(), 
            request.GetValidPageSize(), 
            ct);
    
        var dtos = data.Select(s => new ListarSimulacoesDTO
        {
            Id = s.IdSimulacao,
            ValorDesejado = s.ValorDesejado,
            PrazoMeses = s.PrazoMeses,
            ResultadoSimulacao = s.Resultados?.Select(r => new ValorTotalParcelasDTO
            {
                Tipo = r.Tipo,
                ValorTotal = r.ValorTotal
            }).ToList() ?? new List<ValorTotalParcelasDTO>()
        }).ToList();

        var totalPages = (int)Math.Ceiling((double)totalRecords / request.GetValidPageSize());

        return new PagedResponse<ListarSimulacoesDTO>
        {
            Data = dtos,
            TotalRecords = totalRecords,
            PageNumber = request.GetValidPageNumber(),
            PageSize = request.GetValidPageSize(),
            TotalPages = totalPages
        };
    }
    
    private static Simulacao CriarSimulacao(SimulacaoRequestDTO request, Produto produto)
    {
        return new Simulacao
        {
            CodigoProduto = produto.Codigo,
            DescricaoProduto = produto.Descricao,
            TaxaJuros = produto.TaxaMensal,
            PrazoMeses = (short)request.Prazo,
            ValorDesejado = request.Valor,
            DataReferencia = DateOnly.FromDateTime(DateTime.Today)
        };
    }
    
    private static SimulacaoResponseDTO CriarResponseDto(Simulacao simulacao)
    {
        return new SimulacaoResponseDTO
        {
            Id = simulacao.IdSimulacao,
            CodigoProduto = simulacao.CodigoProduto,
            DescricaoProduto = simulacao.DescricaoProduto,
            TaxaJuros = simulacao.TaxaJuros,
            ResultadoSimulacao = simulacao.Resultados.Select(r => new ResultadoSimulacaoDTO
            {
                Tipo = r.Tipo,
                Parcelas = r.Parcelas.Select(p => new ParcelaDTO
                {
                    Numero = p.Numero,
                    ValorAmortizacao = p.ValorAmortizacao,
                    ValorJuros = p.ValorJuros,
                    ValorPrestacao = p.ValorPrestacao
                }).ToList()
            }).ToList()
        };
    }
}
