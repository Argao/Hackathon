using Hackathon.API.Middleware;
using Hackathon.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Reflection;

namespace Hackathon.API.Tests.Middleware;

public class DatabaseInitializationMiddlewareTests
{
    private void ResetStaticState()
    {
        var field = typeof(DatabaseInitializationMiddleware).GetField("_isInitialized", 
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, false);
    }

    [Fact]
    public async Task InvokeAsync_DeveInicializarBancoDeDadosNaPrimeiraRequisicao()
    {
        // Arrange
        ResetStaticState();
        
        var mockNext = new Mock<RequestDelegate>();
        var mockDbInitializer = new Mock<IDatabaseInitializationService>();
        var mockLogger = new Mock<ILogger<DatabaseInitializationMiddleware>>();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        mockDbInitializer.Setup(x => x.InitializeDatabaseAsync()).Returns(Task.CompletedTask);
        
        var services = new ServiceCollection();
        services.AddSingleton(mockDbInitializer.Object);
        services.AddSingleton(mockLogger.Object);
        services.AddSingleton(mockEnvironment.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        
        var middleware = new DatabaseInitializationMiddleware(
            mockNext.Object, 
            serviceProvider, 
            mockEnvironment.Object);
        
        var context = new DefaultHttpContext();
        context.RequestServices = serviceProvider;
        
        // Act
        await middleware.InvokeAsync(context);
        
        // Assert
        mockDbInitializer.Verify(x => x.InitializeDatabaseAsync(), Times.Once);
        mockNext.Verify(x => x(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_NaoDeveInicializarBancoDeDadosNaSegundaRequisicao()
    {
        // Arrange
        ResetStaticState();
        
        var mockNext = new Mock<RequestDelegate>();
        var mockDbInitializer = new Mock<IDatabaseInitializationService>();
        var mockLogger = new Mock<ILogger<DatabaseInitializationMiddleware>>();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        mockDbInitializer.Setup(x => x.InitializeDatabaseAsync()).Returns(Task.CompletedTask);
        
        var services = new ServiceCollection();
        services.AddSingleton(mockDbInitializer.Object);
        services.AddSingleton(mockLogger.Object);
        services.AddSingleton(mockEnvironment.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        
        var middleware = new DatabaseInitializationMiddleware(
            mockNext.Object, 
            serviceProvider, 
            mockEnvironment.Object);
        
        var context = new DefaultHttpContext();
        context.RequestServices = serviceProvider;
        
        // Act - Primeira chamada
        await middleware.InvokeAsync(context);
        
        // Act - Segunda chamada
        await middleware.InvokeAsync(context);
        
        // Assert
        mockDbInitializer.Verify(x => x.InitializeDatabaseAsync(), Times.Once);
        mockNext.Verify(x => x(context), Times.Exactly(2));
    }

    [Fact]
    public async Task InvokeAsync_DeveInicializarBancoDeDadosEmProducaoNaPrimeiraRequisicao()
    {
        // Arrange
        ResetStaticState();
        
        var mockNext = new Mock<RequestDelegate>();
        var mockDbInitializer = new Mock<IDatabaseInitializationService>();
        var mockLogger = new Mock<ILogger<DatabaseInitializationMiddleware>>();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        mockDbInitializer.Setup(x => x.InitializeDatabaseAsync()).Returns(Task.CompletedTask);
        
        var services = new ServiceCollection();
        services.AddSingleton(mockDbInitializer.Object);
        services.AddSingleton(mockLogger.Object);
        services.AddSingleton(mockEnvironment.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        
        var middleware = new DatabaseInitializationMiddleware(
            mockNext.Object, 
            serviceProvider, 
            mockEnvironment.Object);
        
        var context = new DefaultHttpContext();
        context.RequestServices = serviceProvider;
        
        // Act
        await middleware.InvokeAsync(context);
        
        // Assert
        mockDbInitializer.Verify(x => x.InitializeDatabaseAsync(), Times.Once);
        mockNext.Verify(x => x(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_DeveContinuarEmDesenvolvimentoQuandoErroOcorre()
    {
        // Arrange
        ResetStaticState();
        
        var mockNext = new Mock<RequestDelegate>();
        var mockDbInitializer = new Mock<IDatabaseInitializationService>();
        var mockLogger = new Mock<ILogger<DatabaseInitializationMiddleware>>();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        mockDbInitializer.Setup(x => x.InitializeDatabaseAsync())
            .ThrowsAsync(new InvalidOperationException("Erro na inicialização"));
        
        var services = new ServiceCollection();
        services.AddSingleton(mockDbInitializer.Object);
        services.AddSingleton(mockLogger.Object);
        services.AddSingleton(mockEnvironment.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        
        var middleware = new DatabaseInitializationMiddleware(
            mockNext.Object, 
            serviceProvider, 
            mockEnvironment.Object);
        
        var context = new DefaultHttpContext();
        context.RequestServices = serviceProvider;
        
        // Act
        await middleware.InvokeAsync(context);
        
        // Assert
        mockDbInitializer.Verify(x => x.InitializeDatabaseAsync(), Times.Once);
        mockNext.Verify(x => x(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_DeveFalharEmProducaoQuandoErroOcorre()
    {
        // Arrange
        ResetStaticState();
        
        var mockNext = new Mock<RequestDelegate>();
        var mockDbInitializer = new Mock<IDatabaseInitializationService>();
        var mockLogger = new Mock<ILogger<DatabaseInitializationMiddleware>>();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        
        mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        mockDbInitializer.Setup(x => x.InitializeDatabaseAsync())
            .ThrowsAsync(new InvalidOperationException("Erro na inicialização"));
        
        var services = new ServiceCollection();
        services.AddSingleton(mockDbInitializer.Object);
        services.AddSingleton(mockLogger.Object);
        services.AddSingleton(mockEnvironment.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        
        var middleware = new DatabaseInitializationMiddleware(
            mockNext.Object, 
            serviceProvider, 
            mockEnvironment.Object);
        
        var context = new DefaultHttpContext();
        context.RequestServices = serviceProvider;
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            middleware.InvokeAsync(context));
        
        mockDbInitializer.Verify(x => x.InitializeDatabaseAsync(), Times.Once);
        mockNext.Verify(x => x(context), Times.Never);
    }
}
