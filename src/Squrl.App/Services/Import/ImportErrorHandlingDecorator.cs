using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Import;

public class ImportErrorHandlingDecorator : IImportService
{
    private readonly IImportService _importService;

    public ImportErrorHandlingDecorator(IImportService importService)
    {
        _importService = importService;
    }
    
    public async Task<Result<ImportInfo>> ImportSuppliersAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<ImportInfo> result = await _importService
                .ImportSuppliersAsync(csv, cancellationToken);
            
            return result.Exception is null
                ? result
                : result.WithoutException();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Result<ImportInfo>
                .Fail("Failed to import suppliers, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}