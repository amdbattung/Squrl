using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;

namespace Squrl.App.Services.UnitOfMeasure;

public partial class UomService : IUomService
{
    public async Task<Result<bool>> ExportUomAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<Models.UnitOfMeasure> existingUoms = _dataContext.UnitOfMeasures
            .AsNoTracking()
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Id)
            .AsAsyncEnumerable();
        
        await _csvExporter.ExportAsync(existingUoms, destination, cancellationToken);

        return Result<bool>.Ok(true);
    }
}