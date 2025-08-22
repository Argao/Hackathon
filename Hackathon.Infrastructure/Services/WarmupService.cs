using Hackathon.Domain.Interfaces.Repositories;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Services;
using Hackathon.Domain.Interfaces.Services;
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 Parando serviço de warm-up");
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🚀 Iniciando warm-up da aplicação...");
        var startTime = DateTime.UtcNow;

        try
        {
            // Cada tarefa terá seu próprio escopo de serviço
            await Task.WhenAll(
                WarmupAppDbContext(cancellationToken),
                WarmupProdutoDbContext(cancellationToken),
                WarmupProdutoCache(cancellationToken),
                WarmupEventHub(cancellationToken)
            );

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

    private async Task WarmupAppDbContext(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            _logger.LogDebug("🔥 Warm-up AppDbContext iniciado");

            // OTIMIZAÇÃO: Executar queries que serão usadas na primeira requisição
            await appContext.Simulacoes.CountAsync(cancellationToken);
            await appContext.Metricas.AnyAsync(cancellationToken);

            // Query complexa para forçar compilação de includes (usada em ListarSimulacoes)
            var _ = await appContext.Simulacoes
                .Include(s => s.Resultados)
                .ThenInclude(r => r.Parcelas)
                .OrderByDescending(s => s.DataReferencia)
                .ThenByDescending(s => s.IdSimulacao)
                .Take(1)
                .ToListAsync(cancellationToken);

            // OTIMIZAÇÃO: Query para volume simulado (usada em ObterVolumePorDia)
            var __ = await appContext.Simulacoes
                .Include(s => s.Resultados)
                .ThenInclude(r => r.Parcelas)
                .Where(s => s.DataReferencia == DateOnly.FromDateTime(DateTime.Today))
                .Take(1)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("✅ AppDbContext warm-up concluído");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha no warm-up do AppDbContext");
        }
    }


    private async Task WarmupProdutoDbContext(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
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

    private async Task WarmupProdutoCache(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var produtoService = scope.ServiceProvider.GetRequiredService<ICachedProdutoService>();

            _logger.LogInformation("🔥 OTIMIZAÇÃO: Pré-carregando todos os produtos em cache...");
            var startTime = DateTime.UtcNow;

            // Pré-carrega todos os produtos no cache (estratégia GetAll + filtro em memória)
            var produtos = await produtoService.GetAllAsync(cancellationToken);

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation(
                "✅ Cache otimizado: {Count} produtos carregados em {Duration}ms (próximas buscas: ZERO latência)",
                produtos?.Count() ?? 0, duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha no warm-up do cache de produtos");
        }
    }

    private async Task WarmupEventHub(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var eventHubService = scope.ServiceProvider.GetRequiredService<IEventHubService>();
            // Enviar evento de teste para estabelecer conexão
            await eventHubService.EnviarSimulacaoAsync("warmup", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha no warm-up do EventHub");
        }
    }
}