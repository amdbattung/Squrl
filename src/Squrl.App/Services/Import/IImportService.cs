using Squrl.App.Common;

namespace Squrl.App.Services.Import;

public interface IImportService
{
    Task<Result<ImportInfo>> ImportItemsFromCsvAsync(
        Stream csv,
        CancellationToken cancellationToken = default);
    
    Task<Result<ImportInfo>> ImportUomsFromCsvAsync(
        Stream csv,
        CancellationToken cancellationToken = default);
    
    Task<Result<ImportInfo>> ImportSuppliersFromCsvAsync(
        Stream csv,
        CancellationToken cancellationToken = default);
}