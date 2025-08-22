using Hackathon.Domain.Entities;
using Hackathon.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Simulacao> Simulacoes => Set<Simulacao>();
    public DbSet<ResultadoSimulacao> ResultadosSimulacao => Set<ResultadoSimulacao>();
    public DbSet<Parcela> Parcelas => Set<Parcela>();
    public DbSet<MetricaRequisicao> Metricas => Set<MetricaRequisicao>();


    private static readonly Func<AppDbContext, CancellationToken, Task<int>> _countSimulacoesQuery =
        EF.CompileAsyncQuery((AppDbContext context, CancellationToken ct) => 
            context.Simulacoes.Count());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("main");
        
        ConfigureValueObjectConversions(modelBuilder);

        // Configuração da tabela SIMULACAO
        modelBuilder.Entity<Simulacao>(entity =>
        {
            entity.ToTable("SIMULACAO");
            
            entity.HasKey(e => e.IdSimulacao);
            
            entity.HasIndex(e => e.CodigoProduto)
                .HasDatabaseName("IX_SIMULACAO_CO_PRODUTO");
            
            entity.HasIndex(e => e.DataReferencia)
                .HasDatabaseName("IX_SIMULACAO_DT_REFERENCIA");
            
            entity.Property(e => e.IdSimulacao)
                .HasColumnName("ID_SIMULACAO")
                .IsRequired();
            
            entity.Property(e => e.CodigoProduto)
                .HasColumnName("CO_PRODUTO")
                .IsRequired();
            
            entity.Property(e => e.DescricaoProduto)
                .HasColumnName("NO_PRODUTO")
                .HasMaxLength(200)
                .IsRequired();
            
            entity.Property(e => e.TaxaJuros)
                .HasColumnName("PC_TAXA_JUROS")
                .HasColumnType("decimal(10,9)")
                .HasConversion(
                    v => v.Taxa,
                    v => TaxaJuros.Create(v).Value)
                .IsRequired();
            
            entity.Property(e => e.ValorDesejado)
                .HasColumnName("VR_DESEJADO")
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => ValorMonetario.Create(v).Value)
                .IsRequired();
            
            entity.Property(e => e.PrazoMeses)
                .HasColumnName("NU_PRAZO_MESES")
                .IsRequired();
            
            entity.Property(e => e.DataReferencia)
                .HasColumnName("DT_REFERENCIA")
                .IsRequired();
            
            entity.Property(e => e.EnvelopJson)
                .HasColumnName("TX_ENVELOP_JSON")
                .HasColumnType("text")
                .IsRequired();
            
            // Relacionamento com ResultadoSimulacao
            entity.HasMany(e => e.Resultados)
                .WithOne(r => r.Simulacao)
                .HasForeignKey(r => r.IdSimulacao)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da tabela RESULTADO_SIMULACAO
        modelBuilder.Entity<ResultadoSimulacao>(entity =>
        {
            entity.ToTable("RESULTADO_SIMULACAO");
            
            entity.HasKey(e => e.IdResultado);
            
            entity.HasIndex(e => e.IdSimulacao)
                .HasDatabaseName("IX_RESULTADO_SIMULACAO_ID_SIMULACAO");
            
            entity.Property(e => e.IdResultado)
                .HasColumnName("ID_RESULTADO")
                .IsRequired();
            
            entity.Property(e => e.IdSimulacao)
                .HasColumnName("ID_SIMULACAO")
                .IsRequired();
            
            entity.Property(e => e.Tipo)
                .HasColumnName("TP_SISTEMA_AMORTIZACAO")
                .HasConversion<int>()
                .IsRequired();
                
            entity.Property(e => e.ValorTotal)
                .HasColumnName("VR_TOTAL")
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => ValorMonetario.Create(v).Value);
            
            // Relacionamento com Simulacao
            entity.HasOne(e => e.Simulacao)
                .WithMany(s => s.Resultados)
                .HasForeignKey(e => e.IdSimulacao)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamento com Parcelas
            entity.HasMany(e => e.Parcelas)
                .WithOne(p => p.Resultado)
                .HasForeignKey(p => p.IdResultado)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da tabela PARCELA
        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.ToTable("PARCELA");
            
            // Chave composta
            entity.HasKey(e => new { e.IdResultado, e.Numero });
            
            // Índice adicional para consultas por número da parcela
            entity.HasIndex(e => e.Numero)
                .HasDatabaseName("IX_PARCELA_NU_PARCELA");
            
            entity.Property(e => e.IdResultado)
                .HasColumnName("ID_RESULTADO")
                .IsRequired();
            
            entity.Property(e => e.Numero)
                .HasColumnName("NU_PARCELA")
                .IsRequired();
            
            entity.Property(e => e.ValorPrestacao)
                .HasColumnName("VR_PRESTACAO")
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => ValorMonetario.Create(v).Value)
                .IsRequired();
            
            entity.Property(e => e.ValorAmortizacao)
                .HasColumnName("VR_AMORTIZACAO")
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => ValorMonetario.Create(v).Value)
                .IsRequired();
            
            entity.Property(e => e.ValorJuros)
                .HasColumnName("VR_JUROS")
                .HasColumnType("decimal(18,2)")
                .HasConversion(
                    v => v.Valor,
                    v => ValorMonetario.Create(v).Value)
                .IsRequired();
                
            // Relacionamento com ResultadoSimulacao
            entity.HasOne(e => e.Resultado)
                .WithMany(r => r.Parcelas)
                .HasForeignKey(e => e.IdResultado)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da tabela METRICA_REQUISICAO (Telemetria)
        modelBuilder.Entity<MetricaRequisicao>(entity =>
        {
            entity.ToTable("METRICA_REQUISICAO");
            
            entity.HasKey(e => e.Id);
            
            // Índices para performance nas consultas de telemetria
            entity.HasIndex(e => e.DataHora)
                .HasDatabaseName("IX_METRICA_DT_HORA");
            
            entity.HasIndex(e => new { e.NomeApi, e.DataHora })
                .HasDatabaseName("IX_METRICA_API_DATA");
            
            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .IsRequired();
            
            entity.Property(e => e.NomeApi)
                .HasColumnName("NO_API")
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(e => e.Endpoint)
                .HasColumnName("TX_ENDPOINT")
                .HasMaxLength(500)
                .IsRequired();
            
            entity.Property(e => e.TempoRespostaMs)
                .HasColumnName("NU_TEMPO_RESPOSTA_MS")
                .IsRequired();
            
            entity.Property(e => e.Sucesso)
                .HasColumnName("FL_SUCESSO")
                .IsRequired();
            
            entity.Property(e => e.StatusCode)
                .HasColumnName("NU_STATUS_CODE")
                .IsRequired();
            
            entity.Property(e => e.DataHora)
                .HasColumnName("DT_HORA")
                .IsRequired();
            
        });

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            return;
        }

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var entries = ChangeTracker.Entries();
            var parcelas = entries.Where(e => e.Entity is Parcela && e.State == EntityState.Added).Count();
            
            if (parcelas > 10)
            {
                Console.WriteLine($"⚡ BATCH INSERT: {parcelas} parcelas sendo inseridas em lote");
            }
            
            var startTime = DateTime.UtcNow;
            var result = await base.SaveChangesAsync(cancellationToken);
            var duration = DateTime.UtcNow - startTime;
            
            if (duration.TotalMilliseconds > 100)
            {
                Console.WriteLine($"⚠️ SaveChanges demorou {duration.TotalMilliseconds}ms para {result} registros");
            }
            
            return result;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Erro ao salvar alterações no banco: {ex.Message}", ex);
        }
    }

    private static void ConfigureValueObjectConversions(ModelBuilder modelBuilder)
    {
        var volumeEntity = modelBuilder.Entity<VolumeSimuladoAgregado>();
        volumeEntity.HasNoKey();
        
        // Conversão para TaxaJuros
        volumeEntity.Property(e => e.TaxaMediaJuro)
            .HasConversion(
                v => v.Taxa,
                v => TaxaJuros.Create(v).Value);

        // Conversões para ValorMonetario
        volumeEntity.Property(e => e.ValorMedioPrestacao)
            .HasConversion(
                v => v.Valor,
                v => ValorMonetario.Create(v).Value);

        volumeEntity.Property(e => e.ValorTotalDesejado)
            .HasConversion(
                v => v.Valor,
                v => ValorMonetario.Create(v).Value);

        volumeEntity.Property(e => e.ValorTotalCredito)
            .HasConversion(
                v => v.Valor,
                v => ValorMonetario.Create(v).Value);
    }
}
