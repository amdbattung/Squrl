using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Models;

namespace Squrl.App.Services.PurchaseOrder;

public partial class PurchaseOrderService
{
    public async Task<Result<bool>> ExportPoDetailsAsCsvAsync(Guid purchaseOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<PurchaseOrderDetail> existingPoDetails = _dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.Item)
            .Where(p => p.PurchaseOrder.Id == purchaseOrderId)
            .OrderBy(p => p.LineSequence)
            .ThenBy(p => EF.Property<Instant>(p, "DateCreated"))
            .ThenBy(p => p.Id)
            .AsAsyncEnumerable();
        
        await _csvExporter.ExportAsync(existingPoDetails,
            destination,
            $"PO Details For Purchase Order {purchaseOrderId.ToString()}",
            cancellationToken);

        return Result<bool>.Ok(true);
    }
}