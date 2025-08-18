using Hackathon.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.API.Persistence.Produtos;

public class ProdutosDbContext(DbContextOptions<ProdutosDbContext> opts) : DbContext(opts)
{
    public DbSet<Produto> Produtos => Set<Produto>();
    
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Produto>(e =>
        {
            e.ToTable("PRODUTO", "dbo");
            e.HasKey(x => x.Codigo);
            e.Property(x => x.Codigo).HasColumnName("CO_PRODUTO");
            e.Property(x => x.Descricao).HasColumnName("NO_PRODUTO").HasMaxLength(200);
            e.Property(x => x.TaxaMensal).HasColumnName("PC_TAXA_JUROS").HasColumnType("numeric(10,9)");
            e.Property(x => x.MinMeses).HasColumnName("NU_MINIMO_MESES");
            e.Property(x => x.MaxMeses).HasColumnName("NU_MAXIMO_MESES");
            e.Property(x => x.MinValor).HasColumnName("VR_MINIMO").HasColumnType("numeric(18,2)");
            e.Property(x => x.MaxValor).HasColumnName("VR_MAXIMO").HasColumnType("numeric(18,2)");
        });
    }
}