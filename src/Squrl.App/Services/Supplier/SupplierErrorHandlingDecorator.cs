using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.DTOs;

namespace Squrl.App.Services.Supplier;

public class SupplierErrorHandlingDecorator : ISupplierService
{
    private readonly ISupplierService _supplierService;
    
    public SupplierErrorHandlingDecorator(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }
    
    public async Task<Result<GetManySuppliersDto>> GetManySuppliersAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetManySuppliersDto> result = await _supplierService.GetManySuppliersAsync(
                search,
                pageNumber,
                pageSize,
                orderDirection,
                cancellationToken);

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
            return Result<GetManySuppliersDto>
                .Fail("Failed to retrieve suppliers, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSupplierDto> result = await _supplierService
                .CreateSupplierAsync(supplier, cancellationToken);
            
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
            return Result<GetSupplierDto>
                .Fail("Failed to create supplier, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSupplierDto>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSupplierDto> result = await _supplierService
                .GetSupplierByIdAsync(id, cancellationToken);
            
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
            return Result<GetSupplierDto>
                .Fail("Failed to retrieve supplier, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSupplierDto>> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSupplierDto> result = await _supplierService
                .UpdateSupplierAsync(id, supplier, cancellationToken);
            
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
            return Result<GetSupplierDto>
                .Fail("Failed to update supplier, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetSupplierDto>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetSupplierDto> result = await _supplierService
                .DeleteSupplierAsync(id, cancellationToken);
            
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
            return Result<GetSupplierDto>
                .Fail("Failed to delete supplier, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}