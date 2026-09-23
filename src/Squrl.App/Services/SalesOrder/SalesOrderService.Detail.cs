using FluentValidation.Results;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrderDetails.Commands;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Features.SalesOrderDetails.Mapping;
using Squrl.App.Features.SalesOrderDetails.Queries;
using Squrl.App.Models;

namespace Squrl.App.Services.SalesOrder;

public partial class SalesOrderService
{
    public async Task<Result<GetManySoDetailsDto>> GetSoDetailsAsync(
        Guid salesOrderId,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(new GetManySoDetailsQuery(
                SalesOrderId: salesOrderId,
                PageSize: null,
                OrderDirection: orderDirection), cancellationToken);
            
            GetManySoDetailsDto payload = new GetManySoDetailsDto(
                result.PageSize,
                result.PageNumber,
                result.TotalCount,
                result.Value.Select(SoDetailMapper.ToDto).ToList());
            
            return Result<GetManySoDetailsDto>.Ok(payload);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetManySoDetailsDto>.Fail("Failed to retrieve SO details.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSoDetailDto>> AddSoDetailAsync(Guid salesOrderId, CreateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        try
        {
            soDetail.SalesOrderId = salesOrderId;
            
            ValidationResult validationResult = await _createSoDetailValidator.ValidateAsync(soDetail, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return Result<GetSoDetailDto>.Fail(validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }

            SalesOrderDetail? result = await _mediator.Send(new CreateSoDetailCommand(soDetail), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSoDetailDto>.Fail("Failed to create SO detail.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetSoDetailDto>.Ok(SoDetailMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSoDetailDto>.Fail("Failed to create SO detail.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSoDetailDto>> GetSoDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSoDetailDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            SalesOrderDetail? result = await _mediator.Send(new GetSoDetailByIdQuery(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSoDetailDto>.Fail("SO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSoDetailDto>.Ok(SoDetailMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSoDetailDto>.Fail("Failed to retrieve SO detail.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSoDetailDto>> UpdateSoDetailAsync(Guid id, UpdateSoDetailDto soDetail, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSoDetailDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            ValidationResult validationResult = await _updateSoDetailValidator.ValidateAsync(soDetail, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return Result<GetSoDetailDto>.Fail(validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray())
                    .WithFailureType(FailureType.Validation);
            }

            SalesOrderDetail? result = await _mediator
                .Send(new UpdateSoDetailCommand(id, soDetail), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSoDetailDto>.Fail("SO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSoDetailDto>.Ok(SoDetailMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSoDetailDto>.Fail("Failed to update SO detail.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetSoDetailDto>> RemoveSoDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetSoDetailDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }

            SalesOrderDetail? result = await _mediator
                .Send(new DeleteSoDetailCommand(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetSoDetailDto>.Fail("SO detail not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetSoDetailDto>.Ok(SoDetailMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetSoDetailDto>.Fail("Failed to delete SO detail.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
}