using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Exporters;

public sealed class UnitOfMeasureExporter : CsvExporter<UnitOfMeasure>
{
    public UnitOfMeasureExporter() : base(
        new DataColumn<UnitOfMeasure>("Name", x => x.Name),
        new DataColumn<UnitOfMeasure>("Code", x => x.Code),
        new DataColumn<UnitOfMeasure>("Description", x => x.Description))
    {
        
    }
}