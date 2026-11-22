using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Import;

public sealed class ImportLoggingDecorator : IImportService
{
    private readonly IImportService _importService;
    private readonly ILogger<ImportLoggingDecorator> _logger;

    public ImportLoggingDecorator(IImportService importService,
        ILogger<ImportLoggingDecorator> logger)
    {
        _importService = importService;
        _logger = logger;
    }

    public async Task<Result<ImportInfo>> ImportItemsFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        Result<ImportInfo> result;

        try
        {
            result = await _importService.ImportItemsFromCsvAsync(csv, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Import Items From CSV Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Import Items From CSV Request: " +
                                   "{successCount} items imported, {failedCount} failed.",
                result.Value?.SuccessfulRows,
                result.Value?.FailedRows);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Import Items From CSV Request: {Message}",
                    result.Message ?? "Failed to import items.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Import Items From CSV Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to import items, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Import Items From CSV Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to import items, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<ImportInfo>> ImportUomsFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        Result<ImportInfo> result;

        try
        {
            result = await _importService.ImportUomsFromCsvAsync(csv, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Import UOMs From CSV Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Import UOMs From CSV Request: " +
                                   "{successCount} UOMs imported, {failedCount} failed.",
                result.Value?.SuccessfulRows,
                result.Value?.FailedRows);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Import UOMs From CSV Request: {Message}",
                    result.Message ?? "Failed to import UOMs.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Import UOMs From CSV Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to import UOMs, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Import UOMs From CSV Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to import UOMs, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<ImportInfo>> ImportSuppliersFromCsvAsync(Stream csv, CancellationToken cancellationToken = default)
    {
        Result<ImportInfo> result;

        try
        {
            result = await _importService.ImportSuppliersFromCsvAsync(csv, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Import Suppliers From CSV Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Import Suppliers From CSV Request: " +
                                   "{successCount} suppliers imported, {failedCount} failed.",
                result.Value?.SuccessfulRows,
                result.Value?.FailedRows);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Import Suppliers From CSV Request: {Message}",
                    result.Message ?? "Failed to import suppliers.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Import Suppliers From CSV Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to import suppliers, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Import Suppliers From CSV Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to import suppliers, something went wrong.");
        }
        
        return result;
    }
}