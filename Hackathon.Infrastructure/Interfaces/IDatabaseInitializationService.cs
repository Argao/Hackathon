namespace Hackathon.Infrastructure.Interfaces;

/// <summary>
/// Interface para o serviço de inicialização do banco de dados
/// </summary>
public interface IDatabaseInitializationService
{
    /// <summary>
    /// Aplica as migrations pendentes no banco de dados
    /// </summary>
    Task InitializeDatabaseAsync();
}
