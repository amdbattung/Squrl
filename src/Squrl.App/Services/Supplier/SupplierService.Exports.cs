using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;

namespace Squrl.App.Services.Supplier;

public sealed partial class SupplierService
{
    public async Task<Result> ExportSuppliersAsCsvAsync(Stream destination,
        CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Models.Supplier> existingSuppliers = _dataContext.Suppliers
            .AsNoTracking()
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _csvExporter.ExportAsync(existingSuppliers, destination, cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> ExportSuppliersAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Models.Supplier> existingSuppliers = _dataContext.Suppliers
            .AsNoTracking()
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _excelExporter.ExportAsync(existingSuppliers, destination, cancellationToken);

        return Result.Ok();
    }
}