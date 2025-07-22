using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace api.Core.Result;

public readonly record struct Result<TValue>(TValue? Value, ErrorResult? Error, bool isSuccess)
{
    public bool IsFailure => !isSuccess;
    public bool IsSuccess => isSuccess;

    public static Result<TValue> Success(TValue? value) => new(value, default, true);
    public static Result<TValue> Failure(ErrorResult? error) => new(default, error, false);
    public static Result<TValue> Failure<ErrorType>(
        string message,
        ErrorType code
        ) where ErrorType : Enum => new(default, ErrorResult.New(code, message), false);
    public static Result<TValue> Failure<ErrorType>(
        string message,
        ErrorType code,
        params object?[] args
        ) where ErrorType : Enum => new(default, ErrorResult.New(code, message, args), false);
}

public readonly record struct Result(ErrorResult? Error, bool isSuccess)
{
    public bool IsFailure => !isSuccess;
    public bool IsSuccess => isSuccess;

    public static Result Success() => new(default, true);
    public static Result Failure(ErrorResult? error) => new(error, false);
    public static Result Failure<ErrorType>(
        string message,
        ErrorType code
        ) where ErrorType : Enum => new(ErrorResult.New(code, message), false);
    public static Result Failure<ErrorType>(
        string message,
        ErrorType code,
        params object?[] args
        ) where ErrorType : Enum => new(ErrorResult.New(code, message, args), false);
}

public readonly record struct ErrorResult(Enum Code, string Message, params object?[]? Args)
{
    public static ErrorResult New<ErrorType>(
        ErrorType code,
        string message,
        params object?[] args
        ) where ErrorType : Enum => new(code, message, args.Length > 0 ? args : null);

    public string Formatted => Args?.Length > 0 ? string.Format(Message, Args) : Message;
}

public static class ResultExtensions
{
    public static Result<TValue> ToFailedResultOf<TValue>(this Result result) => Result<TValue>.Failure(result.Error);
}