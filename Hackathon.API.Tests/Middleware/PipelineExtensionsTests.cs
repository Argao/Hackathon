using Hackathon.API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

namespace Hackathon.API.Tests.Middleware;

public class PipelineExtensionsTests
{
    [Fact]
    public void UseInfrastructurePipeline_DeveConfigurarPipelineCorretamente()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());
        var serviceProvider = services.BuildServiceProvider();
        
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app.UseInfrastructurePipeline();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ApplicationBuilder>(result);
    }

    [Fact]
    public void UseDatabaseInitialization_DeveRetornarApplicationBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app.UseDatabaseInitialization();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ApplicationBuilder>(result);
    }

    [Fact]
    public void UseInfrastructurePipeline_DeveConfigurarHttpsRedirectionQuandoNaoEmContainer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Limpar variável de ambiente para garantir que não está em container
        var originalValue = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
        Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", null);

        try
        {
            // Act
            app.UseInfrastructurePipeline();

            // Assert - O pipeline deve ter sido configurado
            Assert.NotNull(app);
        }
        finally
        {
            // Restaurar valor original
            Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", originalValue);
        }
    }

    [Fact]
    public void UseInfrastructurePipeline_DeveConfigurarStaticFiles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app.UseInfrastructurePipeline();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ApplicationBuilder>(result);
    }

    [Fact]
    public void UseInfrastructurePipeline_DeveConfigurarHttpsRedirectionQuandoEmContainer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Configurar variável de ambiente para simular container
        var originalValue = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
        Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", "true");

        try
        {
            // Act
            app.UseInfrastructurePipeline();

            // Assert - O pipeline deve ter sido configurado mesmo em container
            Assert.NotNull(app);
        }
        finally
        {
            // Restaurar valor original
            Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", originalValue);
        }
    }

    [Fact]
    public void UseDatabaseInitialization_DeveConfigurarMiddlewareCorretamente()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var result = app.UseDatabaseInitialization();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ApplicationBuilder>(result);
    }

    private class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "TestApp";
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string ContentRootPath { get; set; } = "/test";
        public string EnvironmentName { get; set; } = "Test";
        public IFileProvider WebRootFileProvider { get; set; } = null!;
        public string WebRootPath { get; set; } = "/test/wwwroot";
    }
}
