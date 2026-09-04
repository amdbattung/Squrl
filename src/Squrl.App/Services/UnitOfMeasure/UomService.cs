using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.Commands;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Features.UnitOfMeasures.Mapping;
using Squrl.App.Features.UnitOfMeasures.Queries;

namespace Squrl.App.Services.UnitOfMeasure;

public class UomService : IUomService
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateUomDto> _createUomValidator;
    private readonly IValidator<UpdateUomDto> _updateUomValidator;

    public UomService(IMediator mediator,
        IValidator<CreateUomDto> createUomValidator,
        IValidator<UpdateUomDto> updateUomValidator)
    {
        _mediator = mediator;
        _createUomValidator = createUomValidator;
        _updateUomValidator = updateUomValidator;
    }

    public async Task<Result<GetManyUomsDto>> GetManyUomsAsync(
        string? query = null,
        int? pageNumber = null,
        int? pageSize = null,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageNumber = pageNumber >= 1 ? pageNumber : null;
            pageSize = pageSize is >= 1 and <= 50 ? pageSize : null;
            
            var result = await _mediator.Send(new GetManyUomsQuery(
                query,
                pageNumber,
                pageSize,
                orderDirection), cancellationToken);
            
            GetManyUomsDto payload = new GetManyUomsDto(
                result.PageNumber,
                result.PageSize,
                result.TotalCount,
                result.Value.Select(UomMapper.ToDto).ToList());
            
            return Result<GetManyUomsDto>.Ok(payload);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetManyUomsDto>.Fail("Failed to retrieve UOMs.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetUomDto>> CreateUomAsync(CreateUomDto uom, CancellationToken cancellationToken = default)
    {
        try
        {
            uom.Name = uom.Name?.Trim();
            uom.Code = uom.Code?.Trim();
            uom.Description = string.IsNullOrWhiteSpace(uom.Description) ? null : uom.Description.Trim();
            
            ValidationResult validationResult = await _createUomValidator.ValidateAsync(uom, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return Result<GetUomDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.UnitOfMeasure? result = await _mediator.Send(new CreateUomCommand(uom), cancellationToken);
            
            if (result == null)
            {
                return Result<GetUomDto>.Fail("Failed to create UOM.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetUomDto>.Ok(UomMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetUomDto>.Fail("Failed to create UOM.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetUomDto>> GetUomByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetUomDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.UnitOfMeasure? result = await _mediator.Send(new GetUomByIdQuery(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetUomDto>.Fail("UOM not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetUomDto>.Ok(UomMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetUomDto>.Fail("Failed to retrieve UOM.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetUomDto>> UpdateUomAsync(Guid id, UpdateUomDto uom, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetUomDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            uom.Name = uom.Name?.Trim();
            uom.Code = uom.Code?.Trim();
            uom.Description = string.IsNullOrWhiteSpace(uom.Description) ? null : uom.Description.Trim();
            
            ValidationResult validationResult = await _updateUomValidator.ValidateAsync(uom, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return Result<GetUomDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.UnitOfMeasure? result = await _mediator.Send(new UpdateUomCommand(id, uom), cancellationToken);
            
            if (result == null)
            {
                return Result<GetUomDto>.Fail("UOM not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetUomDto>.Ok(UomMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetUomDto>.Fail("Failed to update UOM.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetUomDto>> DeleteUomAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return Result<GetUomDto>.Fail("Null or invalid ID.")
                    .WithFailureType(FailureType.Validation);
            }
            
            Models.UnitOfMeasure? result = await _mediator.Send(new DeleteUomCommand(id), cancellationToken);
            
            if (result == null)
            {
                return Result<GetUomDto>.Fail("UOM not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            return Result<GetUomDto>.Ok(UomMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetUomDto>.Fail("Failed to delete UOM.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
}