using FluentValidation.Results;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.Commands;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.PurchaseOrderDetails.Mapping;
using Squrl.App.Features.PurchaseOrderDetails.Queries;
using Squrl.App.Models;

namespace Squrl.App.Services.PurchaseOrder;

public partial class PurchaseOrderService
{
    public async Task<Result<GetManyPoDetailsDto>> GetManyPoDetailsAsync(string? query = null, int? pageNumber = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageNumber = pageNumber >= 1 ? pageNumber : null;

            if (pageSize is null && pageNumber is null)
            {
                pageSize = 1000;
            }
            else if (pageSize is not >= 1 and <= 50)
            {
                pageSize = null;
            }
            
            var result = await _mediator.Send(new GetManyPoDetailsQuery(query, pageNumber, pageSize), cancellationToken);
            
            GetManyPoDetailsDto payload = new GetManyPoDetailsDto(
                result.PageSize,
                result.PageNumber,
                result.TotalCount,
                result.Value.Select(PoDetailMapper.ToDto).ToList());
            
            return Result<GetManyPoDetailsDto>.Ok(payload);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetManyPoDetailsDto>.Fail("Failed to retrieve PO details.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPoDetailDto>> AddPoDetailAsync(Guid purchaseOrderId, CreatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        try
        {
            poDetail.PurchaseOrderId = purchaseOrderId;
            
            ValidationResult validationResult = await _createPoDetailValidator.ValidateAsync(poDetail, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return Result<GetPoDetailDto>.Fail(validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            PurchaseOrderDetail? result = await _mediator.Send(new CreatePoDetailCommand(poDetail), cancellationToken);
            
            if (result == null)
            {
                return Result<GetPoDetailDto>.Fail("Failed to create PO detail.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetPoDetailDto>.Ok(PoDetailMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPoDetailDto>.Fail("Failed to create PO detail.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPoDetailDto>> GetPoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetPoDetailDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            PurchaseOrderDetail? result = await _mediator.Send(new GetPoDetailByIdQuery(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetPoDetailDto>.Fail("PO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetPoDetailDto>.Ok(PoDetailMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPoDetailDto>.Fail("Failed to retrieve PO detail.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPoDetailDto>> UpdatePoDetailAsync(Guid id, UpdatePoDetailDto poDetail, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetPoDetailDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            ValidationResult validationResult = await _updatePoDetailValidator.ValidateAsync(poDetail, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return Result<GetPoDetailDto>.Fail(validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            PurchaseOrderDetail? result = await _mediator.Send(new UpdatePoDetailCommand(id, poDetail), cancellationToken);
            
            if (result == null)
            {
                return Result<GetPoDetailDto>.Fail("PO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetPoDetailDto>.Ok(PoDetailMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPoDetailDto>.Fail("Failed to update PO detail.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPoDetailDto>> RemovePoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetPoDetailDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            PurchaseOrderDetail? result = await _mediator.Send(new DeletePoDetailCommand(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetPoDetailDto>.Fail("PO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetPoDetailDto>.Ok(PoDetailMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPoDetailDto>.Fail("Failed to delete PO detail.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
}