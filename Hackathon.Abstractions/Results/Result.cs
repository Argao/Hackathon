namespace Hackathon.Abstractions.Results;

/// <summary>
/// Resultado genérico para operações que podem falhar
/// </summary>
/// <typeparam name="T">Tipo do valor de sucesso</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Valor de sucesso (apenas se IsSuccess for true)
    /// </summary>
    public T? Value { get; }
    
    /// <summary>
    /// Erro (apenas se IsSuccess for false)
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    /// Código do erro
    /// </summary>
    public string? ErrorCode { get; }

    private Result(bool isSuccess, T? value = default, string? error = null, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Cria um resultado de sucesso
    /// </summary>
    public static Result<T> Success(T value) => new(true, value);

    /// <summary>
    /// Cria um resultado de falha
    /// </summary>
    public static Result<T> Failure(string error, string? errorCode = null) => new(false, error: error, errorCode: errorCode);

    /// <summary>
    /// Executa uma função se o resultado for sucesso
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return IsSuccess ? Result<TNew>.Success(mapper(Value!)) : Result<TNew>.Failure(Error!, ErrorCode);
    }

    /// <summary>
    /// Executa uma função assíncrona se o resultado for sucesso
    /// </summary>
    public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper)
    {
        return IsSuccess ? Result<TNew>.Success(await mapper(Value!)) : Result<TNew>.Failure(Error!, ErrorCode);
    }

    /// <summary>
    /// Executa uma ação se o resultado for sucesso
    /// </summary>
    public void OnSuccess(Action<T> action)
    {
        if (IsSuccess)
            action(Value!);
    }

    /// <summary>
    /// Executa uma ação se o resultado for falha
    /// </summary>
    public void OnFailure(Action<string> action)
    {
        if (!IsSuccess)
            action(Error!);
    }
}
