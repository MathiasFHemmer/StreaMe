namespace Streame.Lib.Result;
public readonly record struct Result<TValue, TError>(TValue? Value, TError? Error, bool isSuccess) where TError : IError
{
    public bool IsFailure(out TError error)
    {
        error = Error!;
        return !isSuccess;
    }
    public bool IsSuccess(out TValue value)
    {
        value = Value!;
        return isSuccess;
    }
    public static Result<TValue, TError> Success(TValue? value) => new(value, default, true);
    public static Result<TValue, TError> Failure(TError? error) => new(default, error, false);
}

public readonly record struct Result<TError>(TError? Error, bool isSuccess) where TError : IError
{
    public bool IsFailure(out TError error)
    {
        error = Error!;
        return !isSuccess;
    }
    public bool IsSuccess => isSuccess;

    public static Result<TError> Success() => new(default, true);
    public static Result<TError> Failure(TError? error) => new(error, false);
}

public readonly record struct Result(Error? Error, bool isSuccess)
{
    public static Result Success() => new(null, true);
    public static Result Failure(Error? error) => new(error, false);
    public static Result<TError> Failure<TError>(TError? error) where TError : IError => new(error, false);
}