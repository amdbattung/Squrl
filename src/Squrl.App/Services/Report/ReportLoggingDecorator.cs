using Squrl.App.Common;
using Squrl.App.Enums;

namespace Squrl.App.Services.Report;

public class ReportLoggingDecorator : IReportService
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportLoggingDecorator> _logger;

    public ReportLoggingDecorator(IReportService reportService,
        ILogger<ReportLoggingDecorator> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }
    
    public async Task<Result<bool>> GenerateCustomerItemSalesAsWord(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _reportService.GenerateCustomerItemSalesAsWord(
                customer,
                destination,
                template,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Generate Customer Item Sales Summary As Word Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Generate Customer Item Sales Summary As Word: report successfully generated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Generate Customer Item Sales Summary As Word Request: {Message}",
                    result.Message ?? "Failed to generate report.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Generate Customer Item Sales Summary As Word Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Generate Customer Item Sales Summary As Word Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> GenerateCustomerItemSalesAsPdf(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _reportService.GenerateCustomerItemSalesAsPdf(
                customer,
                destination,
                template,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Generate Customer Item Sales Summary As PDF Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Generate Customer Item Sales Summary As PDF: report successfully generated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Generate Customer Item Sales Summary As PDF Request: {Message}",
                    result.Message ?? "Failed to generate report.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Generate Customer Item Sales Summary As PDF Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Generate Customer Item Sales Summary As PDF Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsWord(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _reportService.GenerateSupplierItemPurchasesAsWord(
                supplierId,
                destination,
                template,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Generate Supplier Item Purchases Summary As Word Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Generate Supplier Item Purchases Summary As Word Request: report successfully generated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Generate Supplier Item Purchases Summary As Word Request: {Message}",
                    result.Message ?? "Failed to generate report.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Generate Supplier Item Purchases Summary As Word Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Generate Supplier Item Purchases Summary As Word Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsPdf(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _reportService.GenerateSupplierItemPurchasesAsPdf(
                supplierId,
                destination,
                template,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Generate Supplier Item Purchases Summary As PDF Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Generate Supplier Item Purchases Summary As PDF Request: report successfully generated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Generate Supplier Item Purchases Summary As PDF Request: {Message}",
                    result.Message ?? "Failed to generate report.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Generate Supplier Item Purchases Summary As PDF Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Generate Supplier Item Purchases Summary As PDF Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> GenerateItemSalesHistoryAsWord(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _reportService.GenerateItemSalesHistoryAsWord(
                itemId,
                destination,
                template,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Generate Item Sales History As Word Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Generate Item Sales History As Word Request: report successfully generated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Generate Item Sales History As Word Request: {Message}",
                    result.Message ?? "Failed to generate report.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Generate Item Sales History As Word Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Generate Item Sales History As Word Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> GenerateItemSalesHistoryAsPdf(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _reportService.GenerateItemSalesHistoryAsPdf(
                itemId,
                destination,
                template,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Generate Item Sales History As PDF Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Generate Item Sales History As PDF Request: report successfully generated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Generate Item Sales History As PDF Request: {Message}",
                    result.Message ?? "Failed to generate report.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Generate Item Sales History As PDF Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Generate Item Sales History As PDF Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to generate report, something went wrong.");
        }
        
        return result;
    }
}