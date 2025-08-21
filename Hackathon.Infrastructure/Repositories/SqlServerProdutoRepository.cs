using System.Data;
using Dapper;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Repositories;

/// <summary>
/// Repositório simples para produtos read-only do banco externo.
/// </summary>
public class SqlServerProdutoRepository : IProdutoRepository
{
    private readonly Func<IDbConnection> _connectionFactory;
    private readonly ILogger<SqlServerProdutoRepository> _logger;

    public SqlServerProdutoRepository(Func<IDbConnection> connectionFactory, ILogger<SqlServerProdutoRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<Produto?> GetProdutoAdequadoAsync(decimal valor, int prazo, CancellationToken ct)
    {
        const string sql = @"
            SELECT TOP (1)
                CO_PRODUTO AS Codigo,
                NO_PRODUTO AS Descricao,
                PC_TAXA_JUROS AS TaxaMensal,
                NU_MINIMO_MESES AS MinMeses,
                NU_MAXIMO_MESES AS MaxMeses,
                VR_MINIMO AS MinValor,
                VR_MAXIMO AS MaxValor
            FROM dbo.PRODUTO 
            WHERE @prazo >= NU_MINIMO_MESES
                AND (NU_MAXIMO_MESES IS NULL OR @prazo <= NU_MAXIMO_MESES)
                AND @valor >= VR_MINIMO
                AND (VR_MAXIMO IS NULL OR @valor <= VR_MAXIMO)
            ORDER BY CO_PRODUTO";
        
        using var connection = _connectionFactory();
        var command = new CommandDefinition(sql, new { valor, prazo }, commandTimeout: 30, cancellationToken: ct);
        return await connection.QueryFirstOrDefaultAsync<Produto>(command);
    }

    public async Task<Produto?> GetByIdAsync(int codigo, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 
                CO_PRODUTO AS Codigo,
                NO_PRODUTO AS Descricao,
                PC_TAXA_JUROS AS TaxaMensal,
                NU_MINIMO_MESES AS MinMeses,
                NU_MAXIMO_MESES AS MaxMeses,
                VR_MINIMO AS MinValor,
                VR_MAXIMO AS MaxValor
            FROM dbo.PRODUTO 
            WHERE CO_PRODUTO = @codigo";

        using var connection = _connectionFactory();
        var command = new CommandDefinition(sql, new { codigo }, commandTimeout: 30, cancellationToken: ct);
        return await connection.QuerySingleOrDefaultAsync<Produto>(command);
    }

    public async Task<IEnumerable<Produto>?> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT 
                CO_PRODUTO AS Codigo,
                NO_PRODUTO AS Descricao,
                PC_TAXA_JUROS AS TaxaMensal,
                NU_MINIMO_MESES AS MinMeses,
                NU_MAXIMO_MESES AS MaxMeses,
                VR_MINIMO AS MinValor,
                VR_MAXIMO AS MaxValor
            FROM dbo.PRODUTO 
            ORDER BY CO_PRODUTO";

        using var connection = _connectionFactory();
        var command = new CommandDefinition(sql, commandTimeout: 30, cancellationToken: ct);
        return await connection.QueryAsync<Produto>(command);
    }
}