using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Exporters;

public sealed class SoDetailCsvExporter : CsvExporter<SalesOrderDetail>
{
    public SoDetailCsvExporter() : base(
        new DataColumn<SalesOrderDetail>("#", x => x.LineSequence),
        new DataColumn<SalesOrderDetail>("Item", x => x.Item.Name),
        new DataColumn<SalesOrderDetail>("Quantity", x => x.Quantity),
        new DataColumn<SalesOrderDetail>("Unit Price", x => x.UnitPrice),
        new DataColumn<SalesOrderDetail>("Total Price", x => x.Quantity * x.UnitPrice))
    {
        
    }
}