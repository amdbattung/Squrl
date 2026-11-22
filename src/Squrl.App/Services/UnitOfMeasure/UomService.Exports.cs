using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;

namespace Squrl.App.Services.UnitOfMeasure;

public sealed partial class UomService : IUomService
{
    public async Task<Result> ExportUomsAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Models.UnitOfMeasure> existingUoms = _dataContext.UnitOfMeasures
            .AsNoTracking()
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _csvExporter.ExportAsync(existingUoms, destination, cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> ExportUomsAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Models.UnitOfMeasure> existingUoms = _dataContext.UnitOfMeasures
            .AsNoTracking()
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _excelExporter.ExportAsync(existingUoms, destination, cancellationToken);

        return Result.Ok();
    }
}