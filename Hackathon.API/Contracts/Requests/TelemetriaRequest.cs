using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hackathon.API.Contracts.Requests;

/// <summary>
/// Request para obter dados de telemetria por data
/// </summary>
/// <remarks>
/// Este contrato define os parâmetros para consultar métricas de telemetria em uma data específica.
/// 
/// **Parâmetros:**
/// - `DataReferencia`: Data de referência para consulta (formato: YYYY-MM-DD)
/// </remarks>
public sealed record TelemetriaRequest(
    [property: JsonPropertyName("dataReferencia")]
    [property: DefaultValue("2025-08-27")]
    [Required(ErrorMessage = "Data de referência é obrigatória")]
    DateOnly DataReferencia = default
)
{
    public TelemetriaRequest() : this(DateOnly.FromDateTime(DateTime.Today)) { }
};
