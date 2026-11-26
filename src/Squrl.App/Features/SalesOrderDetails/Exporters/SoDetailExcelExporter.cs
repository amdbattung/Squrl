using NodaTime;
using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;
using Squrl.App.Services.Settings;

namespace Squrl.App.Features.SalesOrderDetails.Exporters;

public sealed class SoDetailExcelExporter : ExcelExporter<SalesOrderDetail>
{
    public SoDetailExcelExporter(ISettingsService settingsService, IClock clock) : base(
        clock,
        "Sales Order Details",
        new DataColumn<SalesOrderDetail>("#", x => x.LineSequence),
        new DataColumn<SalesOrderDetail>("Item", x => x.Item.Name),
        new DataColumn<SalesOrderDetail>("Quantity", x => x.Quantity),
        new DataColumn<SalesOrderDetail>(
            $"Unit Price ({settingsService.Current.CurrencySymbol})",
            x => x.UnitPrice),
        new DataColumn<SalesOrderDetail>(
            $"Total Price ({settingsService.Current.CurrencySymbol})",
            x => x.Quantity * x.UnitPrice))
    {
        
    }
}