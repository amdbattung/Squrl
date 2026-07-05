using Squrl.App.Common;
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

    public async Task<Result<GetManyUomsDto>> GetManyUomsAsync(string? query = null, int? pageNumber = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        Result<GetManyUomsDto> result = await _uomService.GetManyUomsAsync(query, pageNumber, pageSize, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Many UOMs Request: {result.Value?.Uoms?.Count} UOMs successfully fetched.");
        }
        else if (result.FailureType == FailureType.Exception)
        {
            _logger.LogError(result.Exception, "Get Many UOMs Request: unhandled exception.");
            return Result<GetManyUomsDto>.Fail(result.Message ?? "Failed to retrieve UOMs.")
                .WithFailureType(FailureType.Unexcepted);
        }
        else
        {
            _logger.LogError(result.Exception, "Get Many UOMs Request: failed to fetch UOMs.");
            return Result<GetManyUomsDto>.Fail(result.Message ?? "Failed to retrieve UOMs.")
                .WithFailureType(FailureType.Unexcepted);
        }
        
        return result;
    }

    public async Task<Result<GetUomDto>> CreateUomAsync(CreateUomDto uom, CancellationToken cancellationToken = default)
    {
        Result<GetUomDto> result = await _uomService.CreateUomAsync(uom, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Create UOM Request: UOM {result.Value?.Name} successfully created.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation($"Create UOM Request: failed to create UOM {result.Value?.Name}, invalid UOM.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Create UOM Request: unhandled exception.");
                return Result<GetUomDto>.Fail(result.Message ?? "Failed to create UOM.");
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception, "Create UOM Request: failed to create UOM.");
                break;
            
            default:
                _logger.LogError(result.Exception, "Create UOM Request: failed to create UOM.");
                return Result<GetUomDto>.Fail(result.Message ?? "Failed to create UOM.");
        }
    
        return result;
    }

    public async Task<Result<GetUomDto>> GetUomByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetUomDto> result = await _uomService.GetUomByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get UOM By ID Request: UOM {result.Value?.Name} successfully fetched.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Get UOM By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Get UOM By ID Request: failed to fetch, UOM not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Get UOM By ID Request: unhandled exception.");
                return Result<GetUomDto>.Fail(result.Message ?? "Failed to retrieve UOM.");
            
            default:
                _logger.LogError(result.Exception, "Get UOM By ID Request: failed to fetch UOM.");
                return Result<GetUomDto>.Fail(result.Message ?? "Failed to retrieve UOM.");
        }
        
        return result;
    }

    public async Task<Result<GetUomDto>> UpdateUomAsync(Guid id, UpdateUomDto uom, CancellationToken cancellationToken = default)
    {
        Result<GetUomDto> result = await _uomService.UpdateUomAsync(id, uom, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update UOM Request: UOM {result.Value?.Name} successfully updated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Update UOM Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Update UOM Request: failed to update, UOM not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update UOM Request: unhandled exception.");
                return Result<GetUomDto>.Fail(result.Message ?? "Failed to update UOM.");
            
            default:
                _logger.LogError(result.Exception, "Update UOM Request: failed to update UOM.");
                return Result<GetUomDto>.Fail(result.Message ?? "Failed to update UOM.");
        }
        
        return result;
    }

    public async Task<Result<GetUomDto>> DeleteUomAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetUomDto> result = await _uomService.DeleteUomAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Delete UOM Request: UOM {result.Value?.Name} successfully deleted.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Delete UOM Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Delete UOM Request: failed to update, UOM not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Delete UOM Request: unhandled exception.");
                return Result<GetUomDto>.Fail(result.Message ?? "Failed to delete UOM.");
            
            default:
                _logger.LogError(result.Exception, "Delete UOM Request: failed to update UOM.");
                return Result<GetUomDto>.Fail(result.Message ?? "Failed to delete UOM.");
        }
        
        return result;
    }
}