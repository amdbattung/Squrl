using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Import;

public class ImportLoggingDecorator : IImportService
{
    private readonly IImportService _importService;
    private readonly ILogger<ImportLoggingDecorator> _logger;

    public ImportLoggingDecorator(IImportService importService,
        ILogger<ImportLoggingDecorator> logger)
    {
        _importService = importService;
        _logger = logger;
    }
    
    public async Task<Result<ImportInfo>> ImportSuppliersAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        Result<ImportInfo> result;

        try
        {
            result = await _importService.ImportSuppliersAsync(csv, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Import Suppliers Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Import Suppliers Request: {successCount} suppliers imported, {failedCount} failed.",
                result.Value?.SuccessfulRows,
                result.Value?.FailedRows);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Import Suppliers Request: {Message}",
                    result.Message ?? "Failed to import suppliers.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Import Suppliers Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to import suppliers, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Import Suppliers Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to import suppliers, something went wrong.");
        }
        
        return result;
    }
}