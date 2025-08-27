using Hackathon.Infrastructure.Extensions;
using Hackathon.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;

namespace Hackathon.Infrastructure.Tests.Extensions;

public class ApplicationBuilderExtensionsTests
{
    [Fact]
    public void UseInfrastructureDatabaseInitialization_EmProducao_DeveInicializarBanco()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment { EnvironmentName = "Production" });
        services.AddScoped<IDatabaseInitializationService, TestDatabaseInitializationService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app.UseInfrastructureDatabaseInitialization();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ApplicationBuilder>(result);
    }

    [Fact]
    public void UseInfrastructureDatabaseInitialization_EmDesenvolvimento_DeveInicializarBanco()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment { EnvironmentName = "Development" });
        services.AddScoped<IDatabaseInitializationService, TestDatabaseInitializationService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app.UseInfrastructureDatabaseInitialization();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ApplicationBuilder>(result);
    }

    [Fact]
    public void UseInfrastructureDatabaseInitialization_ComErro_DevePropagarExcecao()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment { EnvironmentName = "Production" });
        services.AddScoped<IDatabaseInitializationService, FailingDatabaseInitializationService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => app.UseInfrastructureDatabaseInitialization());
        Assert.Contains("Erro simulado na inicialização do banco", exception.Message);
    }

    [Fact]
    public void UseInfrastructureDatabaseInitialization_ComErroEmDesenvolvimento_DevePropagarExcecao()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment { EnvironmentName = "Development" });
        services.AddScoped<IDatabaseInitializationService, FailingDatabaseInitializationService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => app.UseInfrastructureDatabaseInitialization());
        Assert.Contains("Erro simulado na inicialização do banco", exception.Message);
    }

    private class TestHostEnvironment : IHostEnvironment
    {
        public string ApplicationName { get; set; } = "TestApp";
        public string EnvironmentName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = "/test";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private class TestDatabaseInitializationService : IDatabaseInitializationService
    {
        public Task InitializeDatabaseAsync()
        {
            return Task.CompletedTask;
        }
    }

    private class FailingDatabaseInitializationService : IDatabaseInitializationService
    {
        public Task InitializeDatabaseAsync()
        {
            throw new InvalidOperationException("Erro simulado na inicialização do banco");
        }
    }
}
