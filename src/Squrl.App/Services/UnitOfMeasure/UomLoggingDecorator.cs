using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.DTOs;

namespace Squrl.App.Services.UnitOfMeasure;

public class UomLoggingDecorator : IUomService
{
    private readonly IUomService _uomService;
    private readonly ILogger<UomLoggingDecorator> _logger;

    public UomLoggingDecorator(IUomService uomService,
        ILogger<UomLoggingDecorator> logger)
    {
        _uomService = uomService;
        _logger = logger;
    }

    public async Task<Result<GetManyUomsDto>> GetManyUomsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManyUomsDto> result;

        try
        {
            result = await _uomService.GetManyUomsAsync(
                search,
                pageNumber,
                pageSize,
                orderDirection,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Many UOMs Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Many UOMs Request: {Count} UOMs successfully fetched.",
                result.Value?.Uoms?.Count);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get Many UOMs Request: {Message}",
                    result.Message ?? "Failed to retrieve UOMs.");
                break;

            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get Many UOMs Request: Unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve UOMs, something went wrong.");

            default:
                _logger.LogError(result.Exception,
                    "Get Many UOMs Request: Unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve UOMs, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetUomDto>> CreateUomAsync(CreateUomDto uom, CancellationToken cancellationToken = default)
    {
        Result<GetUomDto> result;

        try
        {
            result = await _uomService.CreateUomAsync(uom, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create UOM Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Create UOM Request: UOM {Name} successfully created.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
                
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Create UOM Request Request: {Message}",
                    result.Message ?? "Failed to create UOM.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Create UOM Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create UOM, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Create UOM Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create UOM, something went wrong.");
        }
    
        return result;
    }

    public async Task<Result<GetUomDto>> GetUomByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetUomDto> result;

        try
        {
            result = await _uomService.GetUomByIdAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get UOM By ID Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get UOM By ID Request: UOM {Name} successfully fetched.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Get UOM By ID Request: {Message}",
                    result.Message ?? "Failed to retrieve UOM.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Get UOM By ID Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve UOM, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Get UOM By ID Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve UOM, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetUomDto>> UpdateUomAsync(Guid id, UpdateUomDto uom, CancellationToken cancellationToken = default)
    {
        Result<GetUomDto> result;

        try
        {
            result = await _uomService.UpdateUomAsync(id, uom, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update UOM Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update UOM Request: UOM {Name} successfully updated.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Update UOM Request: {Message}",
                    result.Message ?? "Failed to update UOM.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Update UOM Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update supplier, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Update UOM Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update UOM, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetUomDto>> DeleteUomAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetUomDto> result;

        try
        {
            result = await _uomService.DeleteUomAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Delete UOM Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Delete UOM Request: UOM {Name} successfully deleted.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Delete UOM Request: {Message}",
                    result.Message ?? "Failed to delete UOM.");
                break;
            
            case FailureType.Exception:

            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Delete UOM Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete UOM, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Delete UOM Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete UOM, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> ExportUomsAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _uomService.ExportUomsAsCsvAsync(destination, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Export UOMs As CSV Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Export UOMs As CSV Request: UOMs successfully exported.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Export UOMs As CSV Request: {Message}",
                    result.Message ?? "Failed to export UOMs.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Export UOMs As CSV Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to export UOMs, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Export UOMs As CSV Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to export UOMs, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<bool>> ExportUomsAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        Result<bool> result;

        try
        {
            result = await _uomService.ExportUomsAsExcelAsync(destination, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Export UOMs As Excel Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Export UOMs As Excel Request: UOMs successfully exported.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
            
            case FailureType.NotFound:
            
            case FailureType.BusinessLogic:
                _logger.LogInformation(result.Exception,
                    "Export UOMs As Excel Request: {Message}",
                    result.Message ?? "Failed to export UOMs.");
                break;
            
            case FailureType.Exception:
            
            case FailureType.Unexpected:
                _logger.LogError(result.Exception,
                    "Export UOMs As Excel Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to export UOMs, something went wrong.");
                
            default:
                _logger.LogError(result.Exception,
                    "Export UOMs As Excel Request: unhandled failure type {FailureType}.",
                    Enum.GetName(result.FailureType ?? FailureType.Exception));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to export UOMs, something went wrong.");
        }
        
        return result;
    }
}