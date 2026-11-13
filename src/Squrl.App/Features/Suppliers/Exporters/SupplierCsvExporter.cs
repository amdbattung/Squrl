using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Exporters;

public sealed class SupplierCsvExporter : CsvExporter<Supplier>
{
    public SupplierCsvExporter() : base(
        new DataColumn<Supplier>("Name", x => x.Name),
        new DataColumn<Supplier>("Description", x => x.Description))
    {
        
    }
}