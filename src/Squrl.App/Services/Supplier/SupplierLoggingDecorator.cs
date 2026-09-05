using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.DTOs;

namespace Squrl.App.Services.Supplier;

public class SupplierLoggingDecorator : ISupplierService
{
    private readonly ISupplierService _supplierService;
    private readonly ILogger<SupplierLoggingDecorator> _logger;

    public SupplierLoggingDecorator(ISupplierService supplierService,
        ILogger<SupplierLoggingDecorator> logger)
    {
        _supplierService = supplierService;
        _logger = logger;
    }
    
    public async Task<Result<GetManySuppliersDto>> GetManySuppliersAsync(
        string? query = null,
        int? pageNumber = null,
        int? pageSize = null,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManySuppliersDto> result = await _supplierService.GetManySuppliersAsync(
            query,
            pageNumber,
            pageSize,
            orderDirection,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Many Suppliers Request: {result.Value?.Suppliers?.Count} suppliers successfully fetched.");
        }
        else if (result.FailureType == FailureType.Exception)
        {
            _logger.LogError(result.Exception, "Get Many Suppliers Request: unhandled exception.");
            return Result<GetManySuppliersDto>.Fail(result.Message ?? "Failed to retrieve suppliers.")
                .WithFailureType(FailureType.Unexpected);
        }
        else
        {
            _logger.LogError(result.Exception, "Get Many Suppliers Request: failed to fetch suppliers.");
            return Result<GetManySuppliersDto>.Fail(result.Message ?? "Failed to retrieve suppliers.")
                .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        Result<GetSupplierDto> result = await _supplierService.CreateSupplierAsync(supplier, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Create Supplier Request: supplier {result.Value?.Name} successfully created.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation($"Create Supplier Request: failed to create supplier {result.Value?.Name}, invalid supplier.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception, "Create Supplier Request: failed to create supplier.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Create Supplier Request: unhandled exception.");
                return Result<GetSupplierDto>.Fail(result.Message ?? "Failed to create supplier.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Create Supplier Request: failed to create supplier.");
                return Result<GetSupplierDto>.Fail(result.Message ?? "Failed to create supplier.")
                    .WithFailureType(FailureType.Unexpected);
        }
    
        return result;
    }

    public async Task<Result<GetSupplierDto>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSupplierDto> result = await _supplierService.GetSupplierByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Supplier By ID Request: supplier {result.Value?.Name} successfully fetched.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Get Supplier By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Get Supplier By ID Request: failed to fetch, supplier not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Get Supplier By ID Request: unhandled exception.");
                return Result<GetSupplierDto>.Fail(result.Message ?? "Failed to retrieve supplier.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Get Supplier By ID Request: failed to fetch supplier.");
                return Result<GetSupplierDto>.Fail(result.Message ?? "Failed to retrieve supplier.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        Result<GetSupplierDto> result = await _supplierService.UpdateSupplierAsync(id, supplier, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update Supplier Request: supplier {result.Value?.Name} successfully updated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Update Supplier Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Update Supplier Request: failed to update, supplier not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update Supplier Request: unhandled exception.");
                return Result<GetSupplierDto>.Fail(result.Message ?? "Failed to update supplier.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Update Supplier Request: failed to update supplier.");
                return Result<GetSupplierDto>.Fail(result.Message ?? "Failed to update supplier.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSupplierDto> result = await _supplierService.DeleteSupplierAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Delete Supplier Request: supplier {result.Value?.Name} successfully deleted.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Delete Supplier Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Delete Supplier Request: failed to update, supplier not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Delete Supplier Request: unhandled exception.");
                return Result<GetSupplierDto>.Fail(result.Message ?? "Failed to delete supplier.")
                    .WithFailureType(FailureType.Unexpected);
            
            default:
                _logger.LogError(result.Exception, "Delete Supplier Request: failed to update supplier.");
                return Result<GetSupplierDto>.Fail(result.Message ?? "Failed to delete supplier.")
                    .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }
}