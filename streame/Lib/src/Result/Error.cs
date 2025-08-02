namespace Streame.Lib.Result;
public readonly record struct Error(string Code, string Message, IError? CausedBy = null) : IError
{
    public override string ToString() => $"{Code}: {Message} {(CausedBy is not null ? $"(Caused by: {CausedBy})" : string.Empty)}";
}
