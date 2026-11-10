namespace Squrl.App.Infrastructure.DataExporter;

public interface IExcelExporter<T> : IDataExporter<T>
{
    void Export(IEnumerable<T> data, Stream destination, string? title = null);
    
    Task ExportAsync(IAsyncEnumerable<T> data,
        Stream destination,
        string? title = null,
        CancellationToken cancellationToken = default);
}