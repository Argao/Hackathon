using System.ComponentModel.DataAnnotations;
using Hackathon.Domain.ValueObjects;

namespace Hackathon.Application.DTOs.Requests;

public class SimulacaoRequestDTO
{
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Prazo é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Prazo deve ser maior que zero")]
    public int Prazo { get; set; }
}
