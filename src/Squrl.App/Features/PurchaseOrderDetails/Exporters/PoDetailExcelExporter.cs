using NodaTime;
using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;
using Squrl.App.Services.Settings;

namespace Squrl.App.Features.PurchaseOrderDetails.Exporters;

public sealed class PoDetailExcelExporter : ExcelExporter<PurchaseOrderDetail>
{
    public PoDetailExcelExporter(ISettingsService settingsService, IClock clock) : base(
        clock,
        "Purchase Order Details",
        new DataColumn<PurchaseOrderDetail>("#", x => x.LineSequence),
        new DataColumn<PurchaseOrderDetail>("Item", x => x.Item.Name),
        new DataColumn<PurchaseOrderDetail>("Quantity", x => x.Quantity),
        new DataColumn<PurchaseOrderDetail>(
            $"Unit Price ({settingsService.Current.CurrencySymbol})",
            x => x.UnitPrice),
        new DataColumn<PurchaseOrderDetail>(
            $"Total Price ({settingsService.Current.CurrencySymbol})",
            x => x.Quantity * x.UnitPrice))
    {
        
    }
}