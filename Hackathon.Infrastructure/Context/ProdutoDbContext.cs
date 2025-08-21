using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Infrastructure.Context;

/// <summary>
/// DbContext para acessar produtos do SQL Server externo
/// </summary>
public class ProdutoDbContext : DbContext
{
    public ProdutoDbContext(DbContextOptions<ProdutoDbContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração da tabela PRODUTO no SQL Server externo
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("PRODUTO", "dbo");
            
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).ValueGeneratedNever(); // Chave não é auto-increment
            
            entity.Property(e => e.Codigo)
                .HasColumnName("CO_PRODUTO")
                .IsRequired();
            
            entity.Property(e => e.Descricao)
                .HasColumnName("NO_PRODUTO")
                .HasMaxLength(200)
                .IsRequired();
            
            // Conversão para TaxaJuros
            entity.Property(e => e.TaxaMensal)
                .HasColumnName("PC_TAXA_JUROS")
                .HasColumnType("decimal(10,9)")
                .HasConversion(
                    v => v.Taxa,
                    v => TaxaJuros.Create(v).Value)
                .IsRequired();
            
            entity.Property(e => e.MinMeses)
                .HasColumnName("NU_MINIMO_MESES")
                .IsRequired();
            
            entity.Property(e => e.MaxMeses)
                .HasColumnName("NU_MAXIMO_MESES");
            
            // Conversão para ValorMonetario
            entity.Property(e => e.MinValor)
                .HasColumnName("VR_MINIMO")
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => ValorMonetario.Create(v).Value)
                .IsRequired();
            
            // Conversão para ValorMonetario nullable
            entity.Property(e => e.MaxValor)
                .HasColumnName("VR_MAXIMO")
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v != null ? v.Value.Valor : (decimal?)null,
                    v => v != null ? ValorMonetario.Create(v.Value).Value : (ValorMonetario?)null);
        });

        base.OnModelCreating(modelBuilder);
    }
}
