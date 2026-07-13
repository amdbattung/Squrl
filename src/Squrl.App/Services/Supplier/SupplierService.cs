using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.Commands;
using Squrl.App.Features.Suppliers.DTOs;
using Squrl.App.Features.Suppliers.Mapping;
using Squrl.App.Features.Suppliers.Queries;

namespace Squrl.App.Services.Supplier;

public class SupplierService : ISupplierService
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateSupplierDto> _createSupplierValidator;
    private readonly IValidator<UpdateSupplierDto> _updateSupplierValidator;

    public SupplierService(IMediator mediator,
        IValidator<CreateSupplierDto> createSupplierValidator,
        IValidator<UpdateSupplierDto> updateSupplierValidator)
    {
        _mediator = mediator;
        _createSupplierValidator = createSupplierValidator;
        _updateSupplierValidator = updateSupplierValidator;
    }

    public async Task<Result<GetManySuppliersDto>> GetManySuppliersAsync(string? query = null, int? pageNumber = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageNumber = pageNumber >= 1 ? pageNumber : null;
            pageSize = pageSize is >= 1 and <= 50 ? pageSize : null;
            
            var result = await _mediator.Send(new GetManySuppliersQuery(query, pageNumber, pageSize), cancellationToken);
            
            GetManySuppliersDto payload = new GetManySuppliersDto(
                result.PageSize,
                result.PageNumber,
                result.TotalCount,
                result.Value.Select(SupplierMapper.ToDto).ToList());
            
            return Result<GetManySuppliersDto>.Ok(payload);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetManySuppliersDto>.Fail("Failed to retrieve suppliers.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        try
        {
            supplier.Name = supplier.Name?.Trim();
            supplier.Description = string.IsNullOrWhiteSpace(supplier.Description) ? null : supplier.Description.Trim();
            
            ValidationResult validationResult = await _createSupplierValidator.ValidateAsync(supplier, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return Result<GetSupplierDto>.Fail(validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.Supplier? result = await _mediator.Send(new CreateSupplierCommand(supplier), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSupplierDto>.Fail("Failed to create supplier.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetSupplierDto>.Ok(SupplierMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSupplierDto>.Fail("Failed to create supplier.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSupplierDto>> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSupplierDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.Supplier? result = await _mediator.Send(new GetSupplierByIdQuery(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSupplierDto>.Fail("Supplier not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSupplierDto>.Ok(SupplierMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSupplierDto>.Fail("Failed to retrieve supplier.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSupplierDto>> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplier, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSupplierDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            supplier.Name = supplier.Name?.Trim();
            supplier.Description = string.IsNullOrWhiteSpace(supplier.Description) ? null : supplier.Description.Trim();
            
            ValidationResult validationResult = await _updateSupplierValidator.ValidateAsync(supplier, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return Result<GetSupplierDto>.Fail(validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.Supplier? result = await _mediator.Send(new UpdateSupplierCommand(id, supplier), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSupplierDto>.Fail("Supplier not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSupplierDto>.Ok(SupplierMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSupplierDto>.Fail("Failed to update supplier.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSupplierDto>> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSupplierDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.Supplier? result = await _mediator.Send(new DeleteSupplierCommand(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSupplierDto>.Fail("Supplier not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSupplierDto>.Ok(SupplierMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSupplierDto>.Fail("Failed to delete supplier.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
}