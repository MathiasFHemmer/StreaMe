namespace api.Core.Result;

public interface IError
{
    public IError? CausedBy { get; init; } 
}

public readonly record struct Error(string Code, string Message, IError? CausedBy) : IError
{
    public override string ToString() => $"{Code}: {Message}\nCaused by: {CausedBy?.ToString()}";
}

public readonly record struct UnhandledException(string Message, Exception ex, IError? CausedBy) : IError
{
    public override string ToString() => $"{Message}\nCaused by: {CausedBy?.ToString()}";
}