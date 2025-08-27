namespace Hackathon.Application.Interfaces;

public interface ITelemetriaService
{
    Task RegistrarMetricaAsync(
        string nomeApi, 
        string endpoint, 
        long tempoResposta, 
        bool sucesso, 
        int statusCode,
        CancellationToken cancellationToken = default);
}
