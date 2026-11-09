using System.Globalization;
using System.Text;
using nietras.SeparatedValues;

namespace Squrl.App.Infrastructure.DataExporter;

public abstract class CsvExporter<T> : ICsvExporter<T>
{
    private readonly string? _title;
    private readonly DataColumn<T>[] _columns;
    
    protected CsvExporter(params DataColumn<T>[] columns) : this(null, columns)
    {
        
    }

    protected CsvExporter(string? title, params DataColumn<T>[] columns)
    {
        ArgumentNullException.ThrowIfNull(columns);

        if (columns.Length == 0)
        {
            throw new ArgumentException("At least one column must be specified.", nameof(columns));
        }

        if (columns.Any(column => string.IsNullOrWhiteSpace(column.Header)))
        {
            throw new ArgumentException("Column headers cannot be empty.", nameof(columns));
        }

        if (columns.Select(column => column.Header)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() != columns.Length)
        {
            throw new ArgumentException("Column headers must be unique.", nameof(columns));
        }

        _title = title;
        _columns = columns.ToArray();
    }

    public void Export(IEnumerable<T> data, Stream destination, string? title = null)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(destination);
        
        string? effectiveTitle = title ?? _title;
        
        if (!destination.CanWrite)
        {
            throw new ArgumentException("Destination must be writable.", nameof(destination));
        }
        
        using StreamWriter textWriter = new(
            destination,
            new UTF8Encoding(false),
            leaveOpen: true);
        
        if (!string.IsNullOrWhiteSpace(effectiveTitle))
        {
            textWriter.WriteLineAsync(effectiveTitle.AsMemory());
        }

        using SepWriter writer = Sep.New(',')
            .Writer(o => o with
            {
                Escape = true,
                CultureInfo = CultureInfo.InvariantCulture
            })
            .To(textWriter);

        foreach (DataColumn<T> column in _columns)
        {
            writer.Header.Add(column.Header);
        }

        foreach (T item in data)
        {
            using SepWriter.Row row = writer.NewRow();

            foreach (DataColumn<T> column in _columns)
            {
                object? value = item is null
                    ? null
                    : column.Value(item);
                
                row[column.Header].Set(
                    Convert.ToString(
                        value,
                        CultureInfo.InvariantCulture) ?? string.Empty);
            }
        }
    }

    public async Task ExportAsync(IAsyncEnumerable<T> data,
        Stream destination,
        string? title = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(destination);
        
        string? effectiveTitle = title ?? _title;
        
        if (!destination.CanWrite)
        {
            throw new ArgumentException("Destination must be writable.", nameof(destination));
        }
        
        cancellationToken.ThrowIfCancellationRequested();
        
        await using StreamWriter textWriter = new(
            destination,
            new UTF8Encoding(false),
            leaveOpen: true);
        
        if (!string.IsNullOrWhiteSpace(effectiveTitle))
        {
            await textWriter.WriteLineAsync(
                effectiveTitle.AsMemory(),
                cancellationToken);
        }

        await using SepWriter writer = Sep.New(',')
            .Writer(o => o with
            {
                Escape = true,
                CultureInfo = CultureInfo.InvariantCulture
            })
            .To(textWriter);

        foreach (DataColumn<T> column in _columns)
        {
            writer.Header.Add(column.Header);
        }

        await foreach (T item in data.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await using SepWriter.Row row = writer.NewRow();

            foreach (DataColumn<T> column in _columns)
            {
                object? value = item is null
                    ? null
                    : column.Value(item);
                
                row[column.Header].Set(
                    Convert.ToString(
                        value,
                        CultureInfo.InvariantCulture) ?? string.Empty);
            }
        }
        
        await textWriter.FlushAsync(cancellationToken);
    }

    void IDataExporter<T>.Export(IEnumerable<T> data, Stream destination) 
    {
        Export(data, destination);
    }

    async Task IDataExporter<T>.ExportAsync(IAsyncEnumerable<T> data, Stream destination, CancellationToken cancellationToken = default)
    {
        await ExportAsync(data, destination, null, cancellationToken);
    }
}