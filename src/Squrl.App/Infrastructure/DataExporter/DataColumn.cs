namespace Squrl.App.Infrastructure.DataExporter;

public sealed record DataColumn<T>(
    string Header,
    Func<T, object?> Value);