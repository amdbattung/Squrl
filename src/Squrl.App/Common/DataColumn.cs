namespace Squrl.App.Common;

public sealed record DataColumn<T>(
    string Header,
    Func<T, object?> Value);