using NodaTime;
using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Infrastructure.DecimalFormatter;
using Squrl.App.Models;
using Squrl.App.Services.Settings;

namespace Squrl.App.Features.Items.Exporters;

public class ItemExcelExporter : ExcelExporter<Item>
{
    public ItemExcelExporter(ISettingsService settingsService, IClock clock) : base(
        clock,
        "Items",
        new DataColumn<Item>("Item", x => x.Name),
        new DataColumn<Item>("Description", x => x.Description),
        new DataColumn<Item>("On-hand", x => x.Quantity),
        new DataColumn<Item>("UOM", x => $"{x.Uom.Name} ({x.Uom.Code})"),
        new DataColumn<Item>(
            $"Cost ({settingsService.Current.CurrencySymbol})",
            x => x.CostPrice),
        new DataColumn<Item>(
            $"List Price ({settingsService.Current.CurrencySymbol})",
            x => x.ListPrice),
        new DataColumn<Item>(
            $"Retail Price ({settingsService.Current.CurrencySymbol})",
            x => x.RetailPrice),
        new DataColumn<Item>("Locations", x => string.Join(", ", x.Locations)))
    {
        
    }
}