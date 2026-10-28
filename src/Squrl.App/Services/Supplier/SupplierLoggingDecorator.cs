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
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManySuppliersDto> result;
        
        try
        {
            result = await _supplierService.GetManySuppliersAsync(
                search,
                pageNumber,
                pageSize,
                orderDirection,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Many Suppliers Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Many Suppliers Request: {Count} suppliers successfully fetched.",
                result.Value?.Suppliers?.Count);
        }
        else
        {
            _logger.LogError(result.Exception,
                "Get Many Suppliers Request: unhandled exception type {FailureType}.",
                nameof(result.FailureType));
            throw result.Exception
                  ?? new Exception(result.Message ?? "Failed to retrieve suppliers, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        Result<GetSupplierDto> result;

        try
        {
            result = await _supplierService.CreateSupplierAsync(supplier, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create Supplier Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Create Supplier Request: supplier {Name} successfully created.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Create Supplier Request: failed to create supplier {Name}, invalid supplier.",
                    result.Value?.Name);
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Create Supplier Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create supplier, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Create Supplier Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create supplier, something went wrong.");
        }
    
        return result;
    }

    public async Task<Result<GetSupplierDto>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSupplierDto> result;

        try
        {
            result = await _supplierService.GetSupplierByIdAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Supplier By ID Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Supplier By ID Request: supplier {Name} successfully fetched.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Get Supplier By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Get Supplier By ID Request: failed to fetch, supplier not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Get Supplier By ID Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve supplier, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Get Supplier Request By ID: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve supplier, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        Result<GetSupplierDto> result;

        try
        { 
            result = await _supplierService.UpdateSupplierAsync(id, supplier, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Supplier Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update Supplier Request: supplier {Name} successfully updated.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Update Supplier Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Update Supplier Request: failed to update, supplier not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Update Supplier Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update supplier, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Update Supplier Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update supplier, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetSupplierDto> result;

        try
        {
            result = await _supplierService.DeleteSupplierAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Delete Supplier Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Delete Supplier Request: supplier {Name} successfully deleted.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Delete Supplier Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Delete Supplier Request: failed to delete, supplier not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Delete Supplier Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete supplier, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Delete Supplier Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete supplier, something went wrong.");
        }
        
        return result;
    }
}