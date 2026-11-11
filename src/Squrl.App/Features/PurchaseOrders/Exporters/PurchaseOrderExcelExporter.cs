using NodaTime;
using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Exporters;

public class PurchaseOrderExcelExporter : ExcelExporter<PurchaseOrder>
{
    public PurchaseOrderExcelExporter() : base(
        "Purchase Orders",
        new DataColumn<PurchaseOrder>("ID", x => x.Id),
        new DataColumn<PurchaseOrder>("Issued To", x => x.Supplier?.Name),
        new DataColumn<PurchaseOrder>("Invoice No.", x => x.InvoiceNumber),
        new DataColumn<PurchaseOrder>("Date Ordered", x => x.DateOrdered
            .InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault())
            .Date.ToDateOnly()),
        new DataColumn<PurchaseOrder>("Date Required", x => x.DateRequired?
            .InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault())
            .Date.ToDateOnly()),
        new DataColumn<PurchaseOrder>("Date Shipped", x => x.DateShipped?
            .InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault())
            .Date.ToDateOnly()))
    {
        
    }
}