using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Exporters;

public sealed class PoDetailExcelExporter : ExcelExporter<PurchaseOrderDetail>
{
    public PoDetailExcelExporter() : base(
        "Purchase Order Details",
        new DataColumn<PurchaseOrderDetail>("#", x => x.LineSequence),
        new DataColumn<PurchaseOrderDetail>("Item", x => x.Item.Name),
        new DataColumn<PurchaseOrderDetail>("Quantity", x => x.Quantity),
        new DataColumn<PurchaseOrderDetail>("Unit Price", x => x.UnitPrice),
        new DataColumn<PurchaseOrderDetail>("Total Price", x => x.Quantity * x.UnitPrice))
    {
        
    }
}