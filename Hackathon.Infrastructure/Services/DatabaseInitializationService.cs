using Hackathon.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Services;

/// <summary>
/// Serviço responsável por inicializar o banco de dados e aplicar migrations
/// </summary>
public class DatabaseInitializationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(AppDbContext context, ILogger<DatabaseInitializationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Aplica as migrations pendentes no banco de dados
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando aplicação de migrations...");
            
            // Garantir que o diretório do banco existe
            await EnsureDatabaseDirectoryExistsAsync();
            
            await _context.Database.MigrateAsync();
            
            _logger.LogInformation("✓ Migrations aplicadas com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠ Erro ao aplicar migrations: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Garante que o diretório do banco de dados existe
    /// </summary>
    private async Task EnsureDatabaseDirectoryExistsAsync()
    {
        try
        {
            var connectionString = _context.Database.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Connection string não encontrada");
                return;
            }

            // Extrair o caminho do banco da connection string
            var dataSourceIndex = connectionString.IndexOf("Data Source=", StringComparison.OrdinalIgnoreCase);
            if (dataSourceIndex == -1)
            {
                _logger.LogWarning("Data Source não encontrado na connection string");
                return;
            }

            var dataSource = connectionString.Substring(dataSourceIndex + 12); // "Data Source=" tem 12 caracteres
            var dbPath = dataSource.Trim();
            
            // Se o caminho for relativo, converter para absoluto
            if (!Path.IsPathRooted(dbPath))
            {
                dbPath = Path.GetFullPath(dbPath);
            }

            var dbDirectory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
                _logger.LogInformation("✓ Diretório do banco criado: {Path}", dbDirectory);
            }
            else if (!string.IsNullOrEmpty(dbDirectory))
            {
                _logger.LogInformation("✓ Diretório do banco já existe: {Path}", dbDirectory);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠ Erro ao verificar/criar diretório do banco: {Message}", ex.Message);
        }
    }
}
