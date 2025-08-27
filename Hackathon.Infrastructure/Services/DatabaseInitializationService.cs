using Hackathon.Infrastructure.Context;
using Hackathon.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Services;

public class DatabaseInitializationService : IDatabaseInitializationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(AppDbContext context, ILogger<DatabaseInitializationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
    
            
            // Garantir que o diretório do banco existe
            await EnsureDatabaseDirectoryExistsAsync();
            
            await _context.Database.MigrateAsync();
            

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠ Erro ao aplicar migrations: {Message}", ex.Message);
            throw;
        }
    }

    private async Task EnsureDatabaseDirectoryExistsAsync()
    {
        try
        {
            var connectionString = _context.Database.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
    
                return;
            }

            // Extrair o caminho do banco da connection string
            var dataSourceIndex = connectionString.IndexOf("Data Source=", StringComparison.OrdinalIgnoreCase);
            if (dataSourceIndex == -1)
            {
    
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
    
            }
            else if (!string.IsNullOrEmpty(dbDirectory))
            {

            }
        }
        catch (Exception ex)
        {

        }
    }
}
