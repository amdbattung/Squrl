using FluentValidation;
using FluentValidation.Results;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.DTOs;

namespace Squrl.App.Services.Supplier;

public class SupplierPreprocessingDecorator : ISupplierService
{
    private readonly ISupplierService _supplierService;
    private readonly IValidator<CreateSupplierDto> _createSupplierValidator;
    private readonly IValidator<UpdateSupplierDto> _updateSupplierValidator;

    public SupplierPreprocessingDecorator(ISupplierService supplierService,
        IValidator<CreateSupplierDto> createSupplierValidator,
        IValidator<UpdateSupplierDto> updateSupplierValidator)
    {
        _supplierService = supplierService;
        _createSupplierValidator = createSupplierValidator;
        _updateSupplierValidator = updateSupplierValidator;
    }
    
    public async Task<Result<GetManySuppliersDto>> GetManySuppliersAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        search = search?.Trim();
        pageNumber = pageNumber >= 1 ? pageNumber : 1;
        pageSize = pageSize is >= 1 and <= 50 ? pageSize : 10;
        
        Result<GetManySuppliersDto> result = await _supplierService.GetManySuppliersAsync(
            search,
            pageNumber,
            pageSize,
            orderDirection,
            cancellationToken);

        return result;
    }

    public async Task<Result<GetSupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        supplier.Name = supplier.Name?.Trim();
        supplier.Description = string.IsNullOrWhiteSpace(supplier.Description)
            ? null
            : supplier.Description.Trim();
            
        ValidationResult validationResult = await _createSupplierValidator
            .ValidateAsync(supplier, cancellationToken);
            
        if (!validationResult.IsValid)
        {
            return Result<GetSupplierDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSupplierDto> result = await _supplierService
            .CreateSupplierAsync(supplier, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSupplierDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSupplierDto> result = await _supplierService
            .GetSupplierByIdAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSupplierDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
            
        supplier.Name = supplier.Name?.Trim();
        supplier.Description = string.IsNullOrWhiteSpace(supplier.Description)
            ? null
            : supplier.Description.Trim();
            
        ValidationResult validationResult = await _updateSupplierValidator
            .ValidateAsync(supplier, cancellationToken);
            
        if (!validationResult.IsValid)
        {
            return Result<GetSupplierDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSupplierDto> result = await _supplierService
            .UpdateSupplierAsync(id, supplier, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetSupplierDto>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetSupplierDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetSupplierDto> result = await _supplierService
            .DeleteSupplierAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> ExportSuppliersAsCsvAsync(Stream destination,
        CancellationToken cancellationToken = default)
    {
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<bool> result = await _supplierService
            .ExportSuppliersAsCsvAsync(destination, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> ExportSuppliersAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<bool> result = await _supplierService
            .ExportSuppliersAsExcelAsync(destination, cancellationToken);
        
        return result;
    }
}