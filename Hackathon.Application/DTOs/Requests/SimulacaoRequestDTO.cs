using System.ComponentModel.DataAnnotations;

namespace Hackathon.Application.DTOs.Requests;

public class SimulacaoRequestDTO
{
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 9999999999999999.99, ErrorMessage = "Valor deve estar entre 0.01 e 9999999999999999.99")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Prazo é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Prazo deve ser maior que zero")]
    public int Prazo { get; set; }
}
