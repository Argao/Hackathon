using Hackathon.Application.Commands;
using Hackathon.Application.Exceptions;
using Hackathon.Application.Interfaces;
using Hackathon.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hackathon.Application.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ApplicationValidationException ex)
        {
            _logger.LogWarning("Erro de validação em {RequestName}: {Errors}", 
                typeof(TRequest).Name, string.Join("; ", ex.Errors));
            throw;
        }
        catch (BusinessRuleAppException ex)
        {
            _logger.LogWarning("Regra de negócio violada em {RequestName}: {Message} (Código: {RuleCode})",
                typeof(TRequest).Name, ex.Message, ex.RuleCode);
            throw;
        }
        catch (SimulacaoAppException ex)
        {
            _logger.LogWarning("Erro na simulação em {RequestName}: {Message}",
                typeof(TRequest).Name, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🚨 Erro não tratado em {RequestName}: {Message}", 
                typeof(TRequest).Name, ex.Message);
            throw;
        }
    }
}
