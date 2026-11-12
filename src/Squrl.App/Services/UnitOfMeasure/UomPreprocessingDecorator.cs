using FluentValidation;
using FluentValidation.Results;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.DTOs;

namespace Squrl.App.Services.UnitOfMeasure;

public class UomPreprocessingDecorator : IUomService
{
    private readonly IUomService _uomService;
    private readonly IValidator<CreateUomDto> _createUomValidator;
    private readonly IValidator<UpdateUomDto> _updateUomValidator;

    public UomPreprocessingDecorator(IUomService uomService,
        IValidator<CreateUomDto> createUomValidator,
        IValidator<UpdateUomDto> updateUomValidator)
    {
        _uomService = uomService;
        _createUomValidator = createUomValidator;
        _updateUomValidator = updateUomValidator;
    }
    
    public async Task<Result<GetManyUomsDto>> GetManyUomsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        search = search?.Trim();
        pageNumber = pageNumber >= 1 ? pageNumber : 1;
        pageSize = pageSize is >= 1 and <= 50 ? pageSize : 10;
        
        Result<GetManyUomsDto> result = await _uomService.GetManyUomsAsync(
            search,
            pageNumber,
            pageSize,
            orderDirection,
            cancellationToken);

        return result;
    }

    public async Task<Result<GetUomDto>> CreateUomAsync(CreateUomDto uom, CancellationToken cancellationToken = default)
    {
        uom.Name = uom.Name?.Trim();
        uom.Code = uom.Code?.Trim();
        uom.Description = string.IsNullOrWhiteSpace(uom.Description)
            ? null
            : uom.Description.Trim();
            
        ValidationResult validationResult = await _createUomValidator
            .ValidateAsync(uom, cancellationToken);
            
        if (!validationResult.IsValid)
        {
            return Result<GetUomDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetUomDto> result = await _uomService
            .CreateUomAsync(uom, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetUomDto>> GetUomByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetUomDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetUomDto> result = await _uomService
            .GetUomByIdAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetUomDto>> UpdateUomAsync(Guid id, UpdateUomDto uom, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetUomDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
            
        uom.Name = uom.Name?.Trim();
        uom.Code = uom.Code?.Trim();
        uom.Description = string.IsNullOrWhiteSpace(uom.Description)
            ? null
            : uom.Description.Trim();
            
        ValidationResult validationResult = await _updateUomValidator.ValidateAsync(uom, cancellationToken);
            
        if (!validationResult.IsValid)
        {
            return Result<GetUomDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetUomDto> result = await _uomService
            .UpdateUomAsync(id, uom, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetUomDto>> DeleteUomAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetUomDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetUomDto> result = await _uomService
            .DeleteUomAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> ExportUomsAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<bool> result = await _uomService
            .ExportUomsAsCsvAsync(destination, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> ExportUomsAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<bool> result = await _uomService
            .ExportUomsAsExcelAsync(destination, cancellationToken);
        
        return result;
    }
}