namespace Squrl.App.Infrastructure.DataExporter;

public interface IDataExporter<T>
{
    void Export(IEnumerable<T> data, Stream destination);
    Task ExportAsync(IAsyncEnumerable<T> data,
        Stream destination,
        CancellationToken cancellationToken = default);
}