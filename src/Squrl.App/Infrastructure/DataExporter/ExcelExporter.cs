using System.Globalization;
using ClosedXML.Excel;
using Squrl.App.Common;

namespace Squrl.App.Infrastructure.DataExporter;

public abstract class ExcelExporter<T> : IExcelExporter<T>
{
    private readonly string? _title;
    private readonly DataColumn<T>[] _columns;

    protected ExcelExporter(params DataColumn<T>[] columns) : this(null, columns)
    {
    }

    protected ExcelExporter(string? title, params DataColumn<T>[] columns)
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

        if (!destination.CanWrite)
        {
            throw new ArgumentException("Destination must be writable.", nameof(destination));
        }

        using XLWorkbook workbook = new();

        IXLWorksheet worksheet = workbook.Worksheets.Add("Sheet1");
        
        string? effectiveTitle = title ?? _title;

        int headerRow = WriteTitle(worksheet, effectiveTitle);

        WriteHeaders(worksheet, headerRow);

        int currentRow = headerRow + 1;

        foreach (T item in data)
        {
            for (int columnIndex = 0; columnIndex < _columns.Length; columnIndex++)
            {
                object? value = item is null
                    ? null
                    : _columns[columnIndex].Value(item);

                worksheet.Cell(currentRow, columnIndex + 1)
                    .SetValue(XLCellValue.FromObject(
                        value,
                        CultureInfo.InvariantCulture));
            }

            currentRow++;
        }

        FormatWorksheet(worksheet, headerRow, currentRow - 1);

        workbook.SaveAs(destination);
    }

    public async Task ExportAsync(
        IAsyncEnumerable<T> data,
        Stream destination,
        string? title = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(destination);

        if (!destination.CanWrite)
        {
            throw new ArgumentException("Destination must be writable.", nameof(destination));
        }

        cancellationToken.ThrowIfCancellationRequested();

        using XLWorkbook workbook = new();

        IXLWorksheet worksheet = workbook.Worksheets.Add("Sheet1");
        
        string? effectiveTitle = title ?? _title;

        int headerRow = WriteTitle(worksheet, effectiveTitle);

        WriteHeaders(worksheet, headerRow);

        int currentRow = headerRow + 1;

        await foreach (T item in data.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (int columnIndex = 0; columnIndex < _columns.Length; columnIndex++)
            {
                object? value = item is null
                    ? null
                    : _columns[columnIndex].Value(item);

                worksheet.Cell(currentRow, columnIndex + 1)
                    .SetValue(XLCellValue.FromObject(
                        value,
                        CultureInfo.InvariantCulture));
            }

            currentRow++;
        }

        cancellationToken.ThrowIfCancellationRequested();

        FormatWorksheet(worksheet, headerRow, currentRow - 1);
        
        workbook.SaveAs(destination);
    }

    private int WriteTitle(IXLWorksheet worksheet, string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return 1;
        }

        IXLRange titleRange = worksheet.Range(1, 1, 1, _columns.Length);

        titleRange.Merge();

        titleRange.FirstCell().Value = title;

        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        return 3;
    }

    private void WriteHeaders(IXLWorksheet worksheet, int headerRow)
    {
        for (int columnIndex = 0; columnIndex < _columns.Length; columnIndex++)
        {
            worksheet.Cell(headerRow, columnIndex + 1)
                .Value = _columns[columnIndex].Header;
        }
    }

    private void FormatWorksheet(IXLWorksheet worksheet, int headerRow, int lastRow)
    {
        IXLRange headerRange = worksheet.Range(
            headerRow,
            1,
            headerRow,
            _columns.Length);

        headerRange.Style.Font.Bold = true;

        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Freeze the header and any title rows.
        worksheet.SheetView.FreezeRows(headerRow);

        // Enable filtering when there is at least one data row.
        if (lastRow > headerRow)
        {
            worksheet.Range(
                headerRow,
                1,
                lastRow,
                _columns.Length).SetAutoFilter();
        }

        // Automatically adjust column widths.
        worksheet.Columns(1, _columns.Length).AdjustToContents();
    }

    void IDataExporter<T>.Export(IEnumerable<T> data, Stream destination)
    {
        Export(data, destination);
    }

    async Task IDataExporter<T>.ExportAsync(
        IAsyncEnumerable<T> data,
        Stream destination,
        CancellationToken cancellationToken)
    {
        await ExportAsync(data, destination, null, cancellationToken);
    }
}