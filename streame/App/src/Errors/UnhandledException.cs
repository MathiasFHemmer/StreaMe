using Streame.Lib.Result;

namespace Streame.App.Errors;

public readonly record struct UnhandledException(string Message, Exception ex, IError? CausedBy) : IError
{
    public override string ToString() => $"{Message} {(CausedBy is not null ? $"(Caused by: {CausedBy})" : string.Empty)}";
}