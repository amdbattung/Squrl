using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.DTOs;

namespace Squrl.App.Services.UnitOfMeasure;

public class UomErrorHandlingDecorator : IUomService
{
    private readonly IUomService _uomService;

    public UomErrorHandlingDecorator(IUomService uomService)
    {
        _uomService = uomService;
    }
    
    public async  Task<Result<GetManyUomsDto>> GetManyUomsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetManyUomsDto> result = await _uomService.GetManyUomsAsync(
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
            return Result<GetManyUomsDto>
                .Fail("Failed to retrieve UOMs, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetUomDto>> CreateUomAsync(CreateUomDto uom, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetUomDto> result = await _uomService.CreateUomAsync(uom, cancellationToken);

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
            return Result<GetUomDto>
                .Fail("Failed to create UOM, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetUomDto>> GetUomByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetUomDto> result = await _uomService
                .GetUomByIdAsync(id, cancellationToken);

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
            return Result<GetUomDto>
                .Fail("Failed to retrieve UOM, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetUomDto>> UpdateUomAsync(Guid id, UpdateUomDto uom, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetUomDto> result = await _uomService
                .UpdateUomAsync(id, uom, cancellationToken);

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
            return Result<GetUomDto>
                .Fail("Failed to update UOM, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetUomDto>> DeleteUomAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetUomDto> result = await _uomService
                .DeleteUomAsync(id, cancellationToken);

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
            return Result<GetUomDto>
                .Fail("Failed to delete UOM, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> ExportUomsAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _uomService
                .ExportUomsAsCsvAsync(destination, cancellationToken);
            
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
            return Result<bool>
                .Fail("Failed to export UOMs, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<bool>> ExportUomsAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<bool> result = await _uomService
                .ExportUomsAsExcelAsync(destination, cancellationToken);
            
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
            return Result<bool>
                .Fail("Failed to export UOMs, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}