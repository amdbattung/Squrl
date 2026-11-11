using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Exporters;

public sealed class SupplierExcelExporter : ExcelExporter<Supplier>
{
    public SupplierExcelExporter() : base(
        "Suppliers",
        new DataColumn<Supplier>("Name", x => x.Name),
        new DataColumn<Supplier>("Description", x => x.Description))
    {
        
    }
}