namespace Hackathon.Application.Queries;

/// <summary>
/// Query para obter volume simulado por data
/// </summary>
public sealed record ObterVolumeSimuladoQuery(
    DateOnly DataReferencia
);
