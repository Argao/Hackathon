using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hackathon.Infrastructure.Tests.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceCollection _services;

    public ServiceCollectionExtensionsTests()
    {
        _services = new ServiceCollection();
    }

    [Fact]
    public void AddInfrastructure_ComConfiguracaoValida_DeveRegistrarServicos()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _services.AddLogging(); // Adicionar logging
        _services.AddSingleton<IConfiguration>(configuration); // Adicionar configuração

        // Act
        _services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se os serviços principais foram registrados
        serviceProvider.GetService<ISimulacaoRepository>().Should().NotBeNull();
        serviceProvider.GetService<IProdutoRepository>().Should().NotBeNull();
        serviceProvider.GetService<IMetricaRepository>().Should().NotBeNull();
        serviceProvider.GetService<IEventHubService>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_DeveRegistrarDbContexts()
    {
        // Arrange
        var configuration = CreateValidConfiguration();

        // Act
        _services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se os DbContexts foram registrados
        serviceProvider.GetService<AppDbContext>().Should().NotBeNull();
        serviceProvider.GetService<ProdutoDbContext>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_DeveRegistrarRepositorios()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _services.AddLogging(); // Adicionar logging

        // Act
        _services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se os repositórios foram registrados
        serviceProvider.GetService<ISimulacaoRepository>().Should().NotBeNull();
        serviceProvider.GetService<IMetricaRepository>().Should().NotBeNull();
        serviceProvider.GetService<IProdutoRepository>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_DeveRegistrarServicos()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _services.AddLogging(); // Adicionar logging
        _services.AddSingleton<IConfiguration>(configuration); // Adicionar configuração

        // Act
        _services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se os serviços foram registrados
        serviceProvider.GetService<IEventHubService>().Should().NotBeNull();
        
        // Verificar se o WarmupService foi registrado como IHostedService
        var hostedServices = serviceProvider.GetServices<IHostedService>().ToList();
        hostedServices.Should().Contain(x => x.GetType() == typeof(WarmupService));
    }

    [Fact]
    public void AddInfrastructure_ComConfiguracaoInvalida_DeveLancarExcecao()
    {
        // Arrange
        var configuration = CreateInvalidConfiguration();

        // Act & Assert
        FluentActions.Invoking(() => _services.AddInfrastructure(configuration))
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddInfrastructure_ComProdutosDbNulo_DeveLancarExcecao()
    {
        // Arrange
        var configuration = CreateConfigurationWithoutProdutosDb();

        // Act & Assert
        FluentActions.Invoking(() => _services.AddInfrastructure(configuration))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*ProdutosDb*");
    }

    [Fact]
    public void AddInfrastructure_ComLocalDbNulo_DeveLancarExcecao()
    {
        // Arrange
        var configuration = CreateConfigurationWithoutLocalDb();

        // Act & Assert
        FluentActions.Invoking(() => _services.AddInfrastructure(configuration))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*LocalDb*");
    }

    [Fact]
    public void AddInfrastructure_DeveRegistrarValidators()
    {
        // Arrange
        var configuration = CreateValidConfiguration();

        // Act
        _services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se os validators foram registrados
        serviceProvider.GetService<IValidator<RealizarSimulacaoCommand>>().Should().NotBeNull();
        serviceProvider.GetService<IValidator<ListarSimulacoesQuery>>().Should().NotBeNull();
        serviceProvider.GetService<IValidator<ObterVolumeSimuladoQuery>>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_DeveRegistrarCalculadoras()
    {
        // Arrange
        var configuration = CreateValidConfiguration();

        // Act
        _services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se as calculadoras foram registradas
        var calculadoras = serviceProvider.GetServices<ICalculadoraAmortizacao>().ToList();
        calculadoras.Should().HaveCount(2);
        calculadoras.Should().Contain(x => x.GetType() == typeof(CalculadoraSAC));
        calculadoras.Should().Contain(x => x.GetType() == typeof(CalculadoraPRICE));
    }

    [Fact]
    public void AddInfrastructure_DeveRegistrarApplicationServices()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        _services.AddLogging(); // Adicionar logging
        _services.AddSingleton<IConfiguration>(configuration); // Adicionar configuração

        // Act
        _services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se os serviços de aplicação foram registrados
        serviceProvider.GetService<ISimulacaoService>().Should().NotBeNull();
        serviceProvider.GetService<ITelemetriaService>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_DeveRegistrarMemoryCache()
    {
        // Arrange
        var configuration = CreateValidConfiguration();

        // Act
        _services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se o memory cache foi registrado
        serviceProvider.GetService<IMemoryCache>().Should().NotBeNull();
    }

    private static IConfiguration CreateValidConfiguration()
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectionStrings:LocalDb"] = "Data Source=:memory:",
            ["ConnectionStrings:ProdutosDb"] = "Server=localhost;Database=Produtos;Trusted_Connection=true;",
            ["ConnectionStrings:EventHub"] = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test;EntityPath=testhub"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
    }

    private static IConfiguration CreateInvalidConfiguration()
    {
        var configDict = new Dictionary<string, string?>();

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
    }

    private static IConfiguration CreateConfigurationWithoutProdutosDb()
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectionStrings:LocalDb"] = "Data Source=:memory:",
            ["ConnectionStrings:EventHub"] = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test;EntityPath=testhub"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
    }

    private static IConfiguration CreateConfigurationWithoutLocalDb()
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectionStrings:ProdutosDb"] = "Server=localhost;Database=Produtos;Trusted_Connection=true;",
            ["ConnectionStrings:EventHub"] = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test;EntityPath=testhub"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
    }
}