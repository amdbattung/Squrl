using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;

namespace Squrl.App.Services.Supplier;

public partial class SupplierService
{
    public async Task<Result<bool>> ExportSuppliersAsCsvAsync(Stream destination,
        CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Models.Supplier> existingSuppliers = _dataContext.Suppliers
            .AsNoTracking()
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _csvExporter.ExportAsync(existingSuppliers, destination, cancellationToken);

        return Result<bool>.Ok(true);
    }
}