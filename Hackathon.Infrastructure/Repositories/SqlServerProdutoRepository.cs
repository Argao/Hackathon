using System.Data;
using Dapper;
using Hackathon.Domain.Entities;
using Hackathon.Domain.Interfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Hackathon.Infrastructure.Repositories;

public class SqlServerProdutoRepository : IProdutoRepository
{
    private readonly Func<IDbConnection> _connectionFactory;

    public SqlServerProdutoRepository(Func<IDbConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Produto?> GetProdutoAdequadoAsync(decimal valor, int prazo, CancellationToken ct)
    {
        const string sql = @"
                            SELECT TOP (1)
                              CO_PRODUTO      AS Codigo,
                              NO_PRODUTO      AS Descricao,
                              PC_TAXA_JUROS   AS TaxaMensal,
                              NU_MINIMO_MESES AS MinMeses,
                              NU_MAXIMO_MESES AS MaxMeses,
                              VR_MINIMO       AS MinValor,
                              VR_MAXIMO       AS MaxValor
                            FROM dbo.PRODUTO
                            WHERE @prazo >= NU_MINIMO_MESES
                              AND (NU_MAXIMO_MESES IS NULL OR @prazo <= NU_MAXIMO_MESES)
                              AND @valor >= VR_MINIMO
                              AND (VR_MAXIMO IS NULL OR @valor <= VR_MAXIMO)
                            ORDER BY CO_PRODUTO;";
        
        using var connection = _connectionFactory();
        var command = new CommandDefinition(sql, new { valor, prazo }, commandTimeout: 30, cancellationToken: ct);
        return await connection.QueryFirstOrDefaultAsync<Produto>(command);
    }
}