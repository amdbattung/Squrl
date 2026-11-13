using NodaTime;
using Squrl.App.Common;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Exporters;

public sealed class UnitOfMeasureExcelExporter : ExcelExporter<UnitOfMeasure>
{
    public UnitOfMeasureExcelExporter(IClock clock) : base(
        clock,
        "Unit of Measures",
        new DataColumn<UnitOfMeasure>("Name", x => x.Name),
        new DataColumn<UnitOfMeasure>("Code", x => x.Code),
        new DataColumn<UnitOfMeasure>("Description", x => x.Description))
    {
        
    }
}