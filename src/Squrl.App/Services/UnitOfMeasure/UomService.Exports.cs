using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;

namespace Squrl.App.Services.UnitOfMeasure;

public partial class UomService : IUomService
{
    public async Task<Result<bool>> ExportUomsAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Models.UnitOfMeasure> existingUoms = _dataContext.UnitOfMeasures
            .AsNoTracking()
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _csvExporter.ExportAsync(existingUoms, destination, cancellationToken);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> ExportUomsAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Models.UnitOfMeasure> existingUoms = _dataContext.UnitOfMeasures
            .AsNoTracking()
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _excelExporter.ExportAsync(existingUoms, destination, cancellationToken);

        return Result<bool>.Ok(true);
    }
}