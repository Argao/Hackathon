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

            var dataSourceIndex = connectionString.IndexOf("Data Source=", StringComparison.OrdinalIgnoreCase);
            if (dataSourceIndex == -1)
            { 
                return;
            }

            var dataSource = connectionString.Substring(dataSourceIndex + 12);
            var dbPath = dataSource.Trim();

            if (!Path.IsPathRooted(dbPath))
            {
                dbPath = Path.GetFullPath(dbPath);
            }

            var dbDirectory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao tentar criar diretório para o banco de dados.");
        }
    }
}
