using Squrl.App.Common;

namespace Squrl.App.Services.Import;

public interface IImportService
{
    Task<Result<bool>> ImportSuppliersAsync(
        Stream csv,
        CancellationToken cancellationToken = default);
}