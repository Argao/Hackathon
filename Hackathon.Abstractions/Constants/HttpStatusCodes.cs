namespace Hackathon.Abstractions.Constants;

/// <summary>
/// Códigos de status HTTP comuns
/// </summary>
public static class HttpStatusCodes
{
    // Sucesso
    public const int OK = 200;
    public const int CREATED = 201;
    public const int ACCEPTED = 202;
    public const int NO_CONTENT = 204;
    
    // Redirecionamento
    public const int MOVED_PERMANENTLY = 301;
    public const int FOUND = 302;
    public const int NOT_MODIFIED = 304;
    
    // Erro do cliente
    public const int BAD_REQUEST = 400;
    public const int UNAUTHORIZED = 401;
    public const int FORBIDDEN = 403;
    public const int NOT_FOUND = 404;
    public const int METHOD_NOT_ALLOWED = 405;
    public const int CONFLICT = 409;
    public const int UNPROCESSABLE_ENTITY = 422;
    public const int TOO_MANY_REQUESTS = 429;
    
    // Erro do servidor
    public const int INTERNAL_SERVER_ERROR = 500;
    public const int NOT_IMPLEMENTED = 501;
    public const int BAD_GATEWAY = 502;
    public const int SERVICE_UNAVAILABLE = 503;
    public const int GATEWAY_TIMEOUT = 504;
}
