using FluentValidation;
using FluentValidation.Results;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;

namespace Squrl.App.Services.Inventory;

public class InventoryPreprocessingDecorator : IInventoryService
{
    private readonly IInventoryService _inventoryService;
    private readonly IValidator<CreateItemDto> _createItemValidator;
    private readonly IValidator<UpdateItemDto> _updateItemValidator;

    public InventoryPreprocessingDecorator(IInventoryService inventoryService,
        IValidator<CreateItemDto> createItemValidator,
        IValidator<UpdateItemDto> updateItemValidator)
    {
        _inventoryService = inventoryService;
        _createItemValidator = createItemValidator;
        _updateItemValidator = updateItemValidator;
    }
    
    public async Task<Result<GetManyItemsDto>> GetManyItemsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        search = search?.Trim();
        pageNumber = pageNumber >= 1 ? pageNumber : 1;
        pageSize = pageSize is >= 1 and <= 50 ? pageSize : 10;
        
        Result<GetManyItemsDto> result = await _inventoryService.GetManyItemsAsync(
            search,
            pageNumber,
            pageSize,
            orderDirection,
            cancellationToken);

        return result;
    }

    public async Task<Result<GetItemDto>> CreateItemAsync(
        CreateItemDto item,
        Stream? photo = null,
        CancellationToken cancellationToken = default)
    {
        item.Name = item.Name?.Trim();
        item.Description = string.IsNullOrWhiteSpace(item.Description)
            ? null
            : item.Description.Trim();
        item.Image = null;
        item.Locations = item.Locations?
            .Select(i => i.Trim())
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .ToList();
            
        ValidationResult validationResult = await _createItemValidator
            .ValidateAsync(item, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Result<GetItemDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }

        if (photo is not null)
        {
            const long maxPhotoSize = 1 * 1024L * 1024L; // 1 MB

            if (!photo.CanRead)
            {
                return Result<GetItemDto>.Fail("Photo is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (photo.CanSeek)
            {
                if (photo.Length > maxPhotoSize)
                {
                    return Result<GetItemDto>.Fail("Photo is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                photo.Position = 0;
            }
        }
        
        Result<GetItemDto> result = await _inventoryService
            .CreateItemAsync(item, photo, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetItemDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetItemDto> result = await _inventoryService
            .GetItemByIdAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetItemDto>> UpdateItemAsync(
        Guid id,
        UpdateItemDto item,
        Stream? photo = null,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetItemDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
            
        item.Name = item.Name?.Trim();
        item.Description = string.IsNullOrWhiteSpace(item.Description)
            ? null
            : item.Description.Trim();
        item.Image = null;
        item.Locations = item.Locations?
            .Select(i => i.Trim())
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .ToList();
        
        ValidationResult validationResult = await _updateItemValidator
            .ValidateAsync(item, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Result<GetItemDto>.Fail(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray())
                .WithFailureType(FailureType.Validation);
        }
            
        if (photo is not null)
        {
            const long maxPhotoSize = 1 * 1024L * 1024L; // 1 MB

            if (!photo.CanRead)
            {
                return Result<GetItemDto>.Fail("Photo is not readable.")
                    .WithFailureType(FailureType.Validation);
            }

            if (photo.CanSeek)
            {
                if (photo.Length > maxPhotoSize)
                {
                    return Result<GetItemDto>.Fail("Photo is too large.")
                        .WithFailureType(FailureType.Validation);
                }

                photo.Position = 0;
            }
        }
        
        Result<GetItemDto> result = await _inventoryService
            .UpdateItemAsync(id, item, photo, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetItemDto>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result<GetItemDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetItemDto> result = await _inventoryService
            .DeleteItemAsync(id, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetItemDto>> AddStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        if (itemId == Guid.Empty)
        {
            return Result<GetItemDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetItemDto> result = await _inventoryService
            .AddStocksAsync(itemId, quantity, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetItemDto>> RemoveStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        if (itemId == Guid.Empty)
        {
            return Result<GetItemDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetItemDto> result = await _inventoryService
            .RemoveStocksAsync(itemId, quantity, cancellationToken);
        
        return result;
    }

    public async Task<Result<GetItemDto>> DynamicStockUpdateAsync(Guid itemId, string quantity, CancellationToken cancellationToken = default)
    {
        if (itemId == Guid.Empty)
        {
            return Result<GetItemDto>.Fail("Null or invalid ID.")
                .WithFailureType(FailureType.Validation);
        }
        
        if (string.IsNullOrWhiteSpace(quantity))
        {
            return Result<GetItemDto>.Fail("Invalid quantity.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<GetItemDto> result = await _inventoryService
            .DynamicStockUpdateAsync(itemId, quantity, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> ExportItemsAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<bool> result = await _inventoryService
            .ExportItemsAsCsvAsync(destination, cancellationToken);
        
        return result;
    }

    public async Task<Result<bool>> ExportItemsAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        if (!destination.CanWrite)
        {
            return Result<bool>.Fail("Destination must be writable.")
                .WithFailureType(FailureType.Validation);
        }
        
        Result<bool> result = await _inventoryService
            .ExportItemsAsExcelAsync(destination, cancellationToken);
        
        return result;
    }
}