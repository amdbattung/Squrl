using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Models;

namespace Squrl.App.Services.SalesOrder;

public sealed partial class SalesOrderService
{
    public async Task<Result> ExportSoDetailsAsCsvAsync(Guid salesOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<SalesOrderDetail> existingSoDetails = _dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.Item)
            .Where(s => s.SalesOrder.Id == salesOrderId)
            .OrderBy(s => s.LineSequence)
            .ThenBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _csvExporter.ExportAsync(existingSoDetails,
            destination,
            cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> ExportSoDetailsAsExcelAsync(Guid salesOrderId, Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<SalesOrderDetail> existingSoDetails = _dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.Item)
            .Where(s => s.SalesOrder.Id == salesOrderId)
            .OrderBy(s => s.LineSequence)
            .ThenBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _excelExporter.ExportAsync(existingSoDetails,
            destination,
            $"SO Details For Sales Order {salesOrderId.ToString()}",
            cancellationToken);

        return Result.Ok();
    }
}