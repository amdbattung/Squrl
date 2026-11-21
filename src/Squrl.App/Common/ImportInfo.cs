namespace Squrl.App.Common;

public sealed class ImportInfo
{
    public int TotalRows { get; init; }
    public int SuccessfulRows { get; init; }
    public int FailedRows { get; init; }
    public IReadOnlyList<ImportError> Errors { get; init; } = [];
}

public sealed class ImportError
{
    public int RowNumber { get; init; }
    public string Message { get; init; } = string.Empty;
}