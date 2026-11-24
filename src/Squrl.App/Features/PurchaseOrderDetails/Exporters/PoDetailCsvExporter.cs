using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Infrastructure.DecimalFormatter;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Exporters;

public class PoDetailCsvExporter : CsvExporter<PurchaseOrderDetail>
{
    public PoDetailCsvExporter(IDecimalFormatter decimalFormatter) : base(
        new DataColumn<PurchaseOrderDetail>("#", x => x.LineSequence),
        new DataColumn<PurchaseOrderDetail>("Item", x => x.Item.Name),
        new DataColumn<PurchaseOrderDetail>("Quantity",
            x => decimalFormatter.FormatQuantity(x.Quantity)),
        new DataColumn<PurchaseOrderDetail>("Unit Price",
            x => decimalFormatter.FormatCurrency(x.UnitPrice)),
        new DataColumn<PurchaseOrderDetail>("Total Price",
            x => decimalFormatter.FormatCurrency(x.Quantity * x.UnitPrice)))
    {
        
    }
}