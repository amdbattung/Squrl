using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Exporters;

public sealed class ItemCsvExporter : CsvExporter<Item>
{
    public ItemCsvExporter() : base(
        new DataColumn<Item>("Item", x => x.Name),
        new DataColumn<Item>("Description", x => x.Description),
        new DataColumn<Item>("On-hand", x => x.Quantity),
        new DataColumn<Item>("UOM", x => x.Uom.Code),
        new DataColumn<Item>("Cost", x => x.CostPrice),
        new DataColumn<Item>("List Price", x => x.ListPrice),
        new DataColumn<Item>("Retail Price", x => x.RetailPrice),
        new DataColumn<Item>("Locations", x => x.RetailPrice))
    {
        
    }
}