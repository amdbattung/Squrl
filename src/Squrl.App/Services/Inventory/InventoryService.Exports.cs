using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Models;

namespace Squrl.App.Services.Inventory;

public partial class InventoryService
{
    public async Task<Result<bool>> ExportItemsAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Item> existingItems = _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .OrderBy(i => EF.Property<Instant>(i, "DateCreated"))
            .ThenBy(i => i.Id)
            .AsAsyncEnumerable();
        
        await _csvExporter.ExportAsync(existingItems,
            destination,
            cancellationToken);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> ExportItemsAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Item> existingItems = _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .OrderBy(i => EF.Property<Instant>(i, "DateCreated"))
            .ThenBy(i => i.Id)
            .AsAsyncEnumerable();
        
        await _excelExporter.ExportAsync(existingItems,
            destination,
            cancellationToken);

        return Result<bool>.Ok(true);
    }
}