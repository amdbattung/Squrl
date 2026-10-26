using System.Globalization;
using System.Text;
using nietras.SeparatedValues;

namespace Squrl.App.Infrastructure.DataExporter;

public class CsvExporter<T> : ICsvExporter<T>
{
    private readonly CsvColumn<T>[] _columns;

    public CsvExporter(params CsvColumn<T>[] columns)
    {
        ArgumentNullException.ThrowIfNull(columns);
        _columns = columns;
    }

    public void Export(IEnumerable<T> data, Stream destination)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(destination);
        
        if (!destination.CanWrite)
        {
            throw new ArgumentException(
                "Destination must be writable.",
                nameof(destination));
        }
        
        using StreamWriter textWriter = new(
            destination,
            new UTF8Encoding(false),
            leaveOpen: true);

        using SepWriter writer = Sep.New(',')
            .Writer(o => o with
            {
                Escape = true,
                CultureInfo = CultureInfo.InvariantCulture
            })
            .To(textWriter);

        foreach (CsvColumn<T> column in _columns)
        {
            writer.Header.Add(column.Header);
        }

        foreach (T item in data)
        {
            using SepWriter.Row row = writer.NewRow();

            foreach (CsvColumn<T> column in _columns)
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
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(destination);
        
        if (!destination.CanWrite)
        {
            throw new ArgumentException(
                "Destination must be writable.",
                nameof(destination));
        }
        
        cancellationToken.ThrowIfCancellationRequested();
        
        await using StreamWriter textWriter = new(
            destination,
            new UTF8Encoding(false),
            leaveOpen: true);

        await using SepWriter writer = Sep.New(',')
            .Writer(o => o with
            {
                Escape = true,
                CultureInfo = CultureInfo.InvariantCulture
            })
            .To(textWriter);

        foreach (CsvColumn<T> column in _columns)
        {
            writer.Header.Add(column.Header);
        }

        await foreach (T item in data.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await using SepWriter.Row row = writer.NewRow();

            foreach (CsvColumn<T> column in _columns)
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
}

public sealed record CsvColumn<T>(string Header, Func<T, object?> Value);