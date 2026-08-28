namespace Kanban.Application.Common;

/// <summary>Resultado de una operación que puede fallar por razones de negocio, en lugar de lanzar excepciones.</summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public bool IsFailure => !IsSuccess;

    /// <summary><c>true</c> cuando el fallo es "recurso no encontrado" (HTTP 404).</summary>
    public bool IsNotFound { get; private init; }

    /// <summary><c>true</c> cuando el fallo es "no tenés permiso" (HTTP 403).</summary>
    public bool IsForbidden { get; private init; }

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(string error, bool notFound = false, bool forbidden = false) =>
        new(false, error) { IsNotFound = notFound, IsForbidden = forbidden };

    public static Result<T> Success<T>(T value) => new(value, true, null);

    public static Result<T> Failure<T>(string error, bool notFound = false, bool forbidden = false) =>
        new(default, false, error) { IsNotFound = notFound, IsForbidden = forbidden };
}

public class Result<T> : Result
{
    public T? Value { get; }

    internal Result(T? value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        Value = value;
    }
}
