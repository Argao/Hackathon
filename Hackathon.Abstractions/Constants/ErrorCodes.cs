namespace Hackathon.Abstractions.Constants;

/// <summary>
/// Códigos de erro padronizados
/// </summary>
public static class ErrorCodes
{
    // Validação
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";
    public const string INVALID_INPUT = "INVALID_INPUT";
    public const string REQUIRED_FIELD = "REQUIRED_FIELD";
    public const string INVALID_FORMAT = "INVALID_FORMAT";
    
    // Recursos não encontrados
    public const string NOT_FOUND = "NOT_FOUND";
    public const string RESOURCE_NOT_FOUND = "RESOURCE_NOT_FOUND";
    
    // Regras de negócio
    public const string BUSINESS_RULE_VIOLATION = "BUSINESS_RULE_VIOLATION";
    public const string INVALID_STATE = "INVALID_STATE";
    public const string OPERATION_NOT_ALLOWED = "OPERATION_NOT_ALLOWED";
    
    // Infraestrutura
    public const string INFRASTRUCTURE_ERROR = "INFRASTRUCTURE_ERROR";
    public const string DATABASE_ERROR = "DATABASE_ERROR";
    public const string EXTERNAL_SERVICE_ERROR = "EXTERNAL_SERVICE_ERROR";
    public const string CACHE_ERROR = "CACHE_ERROR";
    
    // Autenticação e Autorização
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string FORBIDDEN = "FORBIDDEN";
    public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
    
    // Genéricos
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
    public const string TIMEOUT = "TIMEOUT";
    public const string RATE_LIMIT_EXCEEDED = "RATE_LIMIT_EXCEEDED";
}
