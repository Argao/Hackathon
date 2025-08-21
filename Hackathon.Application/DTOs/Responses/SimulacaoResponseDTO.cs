namespace Hackathon.Application.DTOs.Responses;

public class SimulacaoResponseDTO
{
    public Guid Id { get; set; }
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public decimal TaxaJuros { get; set; }
    public ICollection<ResultadoSimulacaoDTO> ResultadoSimulacao { get; set; } = [];
}
