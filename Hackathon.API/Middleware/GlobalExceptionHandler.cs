using System.Net;
using System.Text.Json;
using Hackathon.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Hackathon.API.Middleware;

/// <summary>
/// Middleware para tratamento global de exceções
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        HttpStatusCode statusCode;
        string message;
        object details;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                message = "Erro de validação";
                details = new { errors = validationEx.Errors };
                break;
            
            case SimulacaoException simulacaoEx:
                statusCode = HttpStatusCode.UnprocessableEntity;
                message = simulacaoEx.Message;
                details = new { error = simulacaoEx.Message };
                break;
            
            case ProdutoNotFoundException produtoEx:
                statusCode = HttpStatusCode.NotFound;
                message = produtoEx.Message;
                details = new { error = produtoEx.Message, codigoProduto = produtoEx.CodigoProduto };
                break;
            
            case DomainException domainEx:
                statusCode = HttpStatusCode.BadRequest;
                message = domainEx.Message;
                details = new { error = domainEx.Message };
                break;
            
            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "Erro interno do servidor";
                details = new { error = "Ocorreu um erro inesperado" };
                break;
        }

        response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new
        {
            statusCode = response.StatusCode,
            message = message,
            details = details,
            timestamp = DateTime.UtcNow
        });

        // Log da exceção
        _logger.LogError(exception, "Exceção capturada pelo GlobalExceptionHandler: {Message}", exception.Message);

        await response.WriteAsync(result);
    }
}
