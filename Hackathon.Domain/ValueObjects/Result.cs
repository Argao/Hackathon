namespace Hackathon.Domain.ValueObjects;

/// <summary>
/// Representa um resultado de operação que pode ser sucesso ou falha
/// </summary>
public readonly record struct Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    private Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error ?? string.Empty;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);

    public static implicit operator bool(Result result) => result.IsSuccess;
}

/// <summary>
/// Representa um resultado de operação que pode retornar um valor ou falha
/// </summary>
/// <typeparam name="T">Tipo do valor de retorno</typeparam>
public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public string Error { get; }

    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error ?? string.Empty;
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty);
    public static Result<T> Failure(string error) => new(false, default!, error);

    public static implicit operator bool(Result<T> result) => result.IsSuccess;
    public static implicit operator T(Result<T> result) => result.Value;
}
