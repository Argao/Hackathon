using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Hackathon.API.DTOs;

public class SimulacaoRequest
{
    [SwaggerSchema(Description = "Valor desejado do empréstimo")]
    [DefaultValue(900.00)]
    [JsonPropertyName("valorDesejado")]
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 9999999999999999.99, ErrorMessage = "Valor deve estar entre 0.01 e 9999999999999999.99")]
    public decimal Valor { get; set; }

    [SwaggerSchema(Description = "Prazo em meses")]
    [DefaultValue(5)]
    [JsonPropertyName("prazo")]
    [Required(ErrorMessage = "Prazo é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Prazo deve ser maior que zero")]
    public int Prazo { get; set; }
}
