using System.Data;
using Dapper;
using Hackathon.API.Domain.Models;

namespace Hackathon.API.Persistence.Produtos;

internal sealed record ProdutoRow(
    int Codigo,
    string Descricao,
    decimal TaxaMensal,
    short MinMeses,
    short? MaxMeses,
    decimal MinValor,
    decimal? MaxValor);

public class SqlServerProdutoReadRepository : IProdutoReadRepository
{
     private readonly Func<IDbConnection> _connFactory;

     public SqlServerProdutoReadRepository(Func<IDbConnection> connFactory)
     {
         _connFactory = connFactory;
     }

     public async Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken ct)
    {
        const string sql = @"
                            SELECT 
                              CO_PRODUTO      AS Codigo,
                              NO_PRODUTO      AS Descricao,
                              PC_TAXA_JUROS   AS TaxaMensal,
                              NU_MINIMO_MESES AS MinMeses,
                              NU_MAXIMO_MESES AS MaxMeses,
                              VR_MINIMO       AS MinValor,
                              VR_MAXIMO       AS MaxValor
                            FROM dbo.PRODUTO
                            ORDER BY CO_PRODUTO;";

        using var cn = _connFactory();
        var rows = await cn.QueryAsync<ProdutoRow>(
            new CommandDefinition(sql, cancellationToken: ct, commandTimeout: 30));
        return rows.Select(Map).ToList();
    }

    public async Task<Produto?> SelecionarProdutoParaAsync(decimal valor, int prazo, CancellationToken ct)
    {
        // Seleciona o PRIMEIRO produto cujo range contempla o valor/prazo
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
                              AND (@prazo <= ISNULL(NU_MAXIMO_MESES, @prazo))
                              AND @valor >= VR_MINIMO
                              AND (@valor <= ISNULL(VR_MAXIMO, @valor))
                            ORDER BY CO_PRODUTO;";

        using var cn = _connFactory();
        var row = await cn.QueryFirstOrDefaultAsync<ProdutoRow>(new CommandDefinition(sql, new { valor, prazo },
            cancellationToken: ct, commandTimeout: 30));

        return row is null ? null : Map(row);
    }

    private static Produto Map(ProdutoRow r)
    {
        return new Produto(r.Codigo, r.Descricao, r.TaxaMensal, r.MinMeses, r.MaxMeses, r.MinValor, r.MaxValor);
    }
}