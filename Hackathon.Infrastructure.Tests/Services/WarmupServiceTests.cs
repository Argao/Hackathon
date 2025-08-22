using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hackathon.Application.Services;

namespace Hackathon.Infrastructure.Tests.Services;

public class WarmupServiceTests
{
    [Fact]
    public void Constructor_ComParametrosValidos_DeveCriarInstancia()
    {
        // Arrange
        var serviceProvider = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<WarmupService>>();

        // Act & Assert
        FluentActions.Invoking(() => new WarmupService(serviceProvider.Object, logger.Object))
            .Should().NotThrow();
    }

    [Fact]
    public async Task StartAsync_DeveIniciarServico()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var logger = new Mock<ILogger<WarmupService>>();
        var warmupService = new WarmupService(serviceProvider, logger.Object);
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await FluentActions.Invoking(() => warmupService.StartAsync(cancellationToken))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task StopAsync_DeveFinalizarServico()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var logger = new Mock<ILogger<WarmupService>>();
        var warmupService = new WarmupService(serviceProvider, logger.Object);
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await FluentActions.Invoking(() => warmupService.StopAsync(cancellationToken))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task StartAsync_ComCancellationToken_DeveRespeitarCancelamento()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var logger = new Mock<ILogger<WarmupService>>();
        var warmupService = new WarmupService(serviceProvider, logger.Object);
        
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await FluentActions.Invoking(() => warmupService.StartAsync(cancellationTokenSource.Token))
            .Should().NotThrowAsync(); // O serviço deve lidar com o cancelamento graciosamente
    }

    [Fact]
    public async Task StartAsync_ComServicosIndisponiveis_DeveLoggarEContinuar()
    {
        // Arrange
        var serviceProvider = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<WarmupService>>();
        
        // Setup para retornar null para os serviços
        serviceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(null);
        
        var warmupService = new WarmupService(serviceProvider.Object, logger.Object);
        var cancellationToken = new CancellationToken();

        // Act & Assert
        await FluentActions.Invoking(() => warmupService.StartAsync(cancellationToken))
            .Should().NotThrowAsync();

        // Verificar se logs foram gerados
        logger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    private static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        
        // Adicionar DbContexts em memória
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            
        services.AddDbContext<ProdutoDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        // Adicionar mocks dos serviços
        services.AddSingleton(Mock.Of<IEventHubService>());
        
        return services.BuildServiceProvider();
    }
}