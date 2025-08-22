using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Services;
using Hackathon.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Services;

/// <summary>
/// Serviço de warm-up para pré-carregar DbContexts e compilar queries
/// Resolve o problema de Cold Start da primeira requisição
/// </summary>
public class WarmupService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WarmupService> _logger;

    public WarmupService(IServiceProvider serviceProvider, ILogger<WarmupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🚀 Iniciando warm-up da aplicação...");
        var startTime = DateTime.UtcNow;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            
            // WARM-UP 1: Pré-carregar AppDbContext (SQLite)
            await WarmupAppDbContext(scope, cancellationToken);
            
            // WARM-UP 2: Pré-carregar ProdutoDbContext (SQL Server)  
            await WarmupProdutoDbContext(scope, cancellationToken);
            
            // WARM-UP 3: Pré-carregar cache de produtos
            await WarmupProdutoCache(scope, cancellationToken);

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("✅ Warm-up concluído em {Duration}ms", duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogWarning(ex, "⚠️ Warm-up falhou em {Duration}ms, mas aplicação continuará funcionando", 
                duration.TotalMilliseconds);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 Parando serviço de warm-up");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Warm-up do AppDbContext (SQLite) - compila queries principais
    /// </summary>
    private async Task WarmupAppDbContext(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            _logger.LogDebug("🔥 Warm-up AppDbContext iniciado");
            
            // Força compilação das queries mais usadas
            await appContext.Simulacoes.CountAsync(cancellationToken);
            await appContext.Metricas.AnyAsync(cancellationToken);
            
            // Query complexa para forçar compilação de includes
            var _ = await appContext.Simulacoes
                .Include(s => s.Resultados)
                .ThenInclude(r => r.Parcelas)
                .OrderByDescending(s => s.DataReferencia)
                .Take(1)
                .ToListAsync(cancellationToken);
            
            _logger.LogDebug("✅ AppDbContext warm-up concluído");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha no warm-up do AppDbContext");
        }
    }

    /// <summary>
    /// Warm-up do ProdutoDbContext (SQL Server) - compila queries de produtos
    /// </summary>
    private async Task WarmupProdutoDbContext(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var produtoContext = scope.ServiceProvider.GetRequiredService<ProdutoDbContext>();
            
            _logger.LogDebug("🔥 Warm-up ProdutoDbContext iniciado");
            
            // Força compilação das queries de produto
            await produtoContext.Produtos.CountAsync(cancellationToken);
            await produtoContext.Produtos.OrderBy(p => p.Codigo).Take(1).ToListAsync(cancellationToken);
            
            _logger.LogDebug("✅ ProdutoDbContext warm-up concluído");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha no warm-up do ProdutoDbContext - SQL Server pode não estar disponível");
        }
    }

    /// <summary>
    /// Warm-up do cache de produtos - pré-carrega produtos em memória
    /// OTIMIZAÇÃO: Busca todos os 4 produtos de uma vez e cacheia por 1 hora
    /// </summary>
    private async Task WarmupProdutoCache(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var produtoService = scope.ServiceProvider.GetRequiredService<ICachedProdutoService>();
            
            _logger.LogInformation("🔥 OTIMIZAÇÃO: Pré-carregando todos os produtos em cache...");
            var startTime = DateTime.UtcNow;
            
            // Pré-carrega todos os produtos no cache (estratégia GetAll + filtro em memória)
            var produtos = await produtoService.GetAllAsync(cancellationToken);
            
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("✅ Cache otimizado: {Count} produtos carregados em {Duration}ms (próximas buscas: ZERO latência)", 
                produtos?.Count() ?? 0, duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha no warm-up do cache de produtos");
        }
    }
}
