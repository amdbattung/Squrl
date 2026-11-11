using NodaTime;
using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Exporters;

public sealed class SalesOrderExcelExporter : ExcelExporter<SalesOrder>
{
    public SalesOrderExcelExporter() : base(
        "Sales Orders",
        new DataColumn<SalesOrder>("ID", x => x.Id),
        new DataColumn<SalesOrder>("Issued To", x => x.Customer),
        new DataColumn<SalesOrder>("Invoice No.", x => x.InvoiceNumber),
        new DataColumn<SalesOrder>("Date Ordered", x => x.DateOrdered
            .InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault())
            .Date.ToDateOnly()))
    {
        
    }
}