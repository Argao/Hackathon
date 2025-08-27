using Hackathon.Infrastructure.Extensions;
using Hackathon.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment { EnvironmentName = "Production" });
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
    public void UseInfrastructureDatabaseInitialization_EmDesenvolvimento_NaoDeveInicializarBanco()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment { EnvironmentName = "Development" });
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
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment { EnvironmentName = "Production" });
        services.AddScoped<IDatabaseInitializationService, FailingDatabaseInitializationService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => app.UseInfrastructureDatabaseInitialization());
    }

    private class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "TestApp";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "/test";
        public string EnvironmentName { get; set; } = "Test";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = "/test/wwwroot";
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
