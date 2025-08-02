using Streame.Lib.Result;

namespace Streame.App.Errors;

public readonly record struct InvalidAPIRequest(List<InvalidProperty> InvalidProperties) : IError
{
    public IError? CausedBy { get; init; }
    public void AddInvalidProperty(string propertyName, string message) => InvalidProperties.Add(new InvalidProperty(propertyName, message, null));
    public override string ToString() => $"Invalid API Request: {string.Join(", ", InvalidProperties.Select(p => p.ToString()))} {(CausedBy is not null ? $"(Caused by: {CausedBy})" : string.Empty)}";
}

public readonly record struct InvalidProperty(string PropertyName, string Message, IError? CausedBy = null) : IError 
{
    public override string ToString() => $"{PropertyName}: {Message} {(CausedBy is not null ? $"(Caused by: {CausedBy})" : string.Empty)}";
}
