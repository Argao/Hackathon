namespace Hackathon.Application.DTOs.Responses;

public class VolumeSimuladoResponseDTO
{
    public string DataReferencia { get; set; } = string.Empty;
    public ICollection<VolumeSimuladoProdutoDTO> Simulacoes { get; set; } = [];
}
