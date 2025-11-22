using System.Net;
using System.Text.Json;
using Hackathon.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Hackathon.API.Middleware;

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
            case ApplicationValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                message = "Erro de validação";
                details = new { errors = validationEx.Errors };
                break;

            case SimulacaoAppException simulacaoEx:
                statusCode = HttpStatusCode.UnprocessableEntity;
                message = simulacaoEx.Message;
                details = new { error = simulacaoEx.Message };
                break;

            case NotFoundAppException notFoundEx:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundEx.Message;
                details = new { error = notFoundEx.Message, resourceId = notFoundEx.ResourceId };
                break;

            case BusinessRuleAppException businessRuleEx:
                statusCode = HttpStatusCode.BadRequest;
                message = businessRuleEx.Message;
                details = new { error = businessRuleEx.Message, ruleCode = businessRuleEx.RuleCode };
                break;

            case ApplicationExceptionBase appEx:
                statusCode = HttpStatusCode.BadRequest;
                message = appEx.Message;
                details = new { error = appEx.Message };
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
