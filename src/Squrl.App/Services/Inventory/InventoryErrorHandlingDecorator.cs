using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;

namespace Squrl.App.Services.Inventory;

public class InventoryErrorHandlingDecorator : IInventoryService
{
    private readonly IInventoryService _inventoryService;

    public InventoryErrorHandlingDecorator(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    
    public async Task<Result<GetManyItemsDto>> GetManyItemsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetManyItemsDto> result = await _inventoryService.GetManyItemsAsync(
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
            return Result<GetManyItemsDto>
                .Fail("Failed to retrieve items, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetItemDto>> CreateItemAsync(
        CreateItemDto item,
        Stream? photo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetItemDto> result = await _inventoryService
                .CreateItemAsync(item, photo, cancellationToken);
            
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
            return Result<GetItemDto>
                .Fail("Failed to create item, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetItemDto> result = await _inventoryService
                .GetItemByIdAsync(id, cancellationToken);
            
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
            return Result<GetItemDto>
                .Fail("Failed to retrieve item, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetItemDto>> UpdateItemAsync(
        Guid id,
        UpdateItemDto item,
        Stream? photo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetItemDto> result = await _inventoryService
                .UpdateItemAsync(id, item, photo, cancellationToken);
            
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
            return Result<GetItemDto>
                .Fail("Failed to update item, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetItemDto>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetItemDto> result = await _inventoryService
                .DeleteItemAsync(id, cancellationToken);
            
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
            return Result<GetItemDto>
                .Fail("Failed to delete item, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetItemDto>> AddStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetItemDto> result = await _inventoryService
                .AddStocksAsync(itemId, quantity, cancellationToken);
            
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
            return Result<GetItemDto>
                .Fail("Failed to add stocks, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetItemDto>> RemoveStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetItemDto> result = await _inventoryService
                .RemoveStocksAsync(itemId, quantity, cancellationToken);
            
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
            return Result<GetItemDto>
                .Fail("Failed to remove stocks, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }

    public async Task<Result<GetItemDto>> DynamicStockUpdateAsync(Guid itemId, string quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            Result<GetItemDto> result = await _inventoryService
                .DynamicStockUpdateAsync(itemId, quantity, cancellationToken);
            
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
            return Result<GetItemDto>
                .Fail("Failed to update stocks, something went wrong.")
                .WithFailureType(FailureType.Unexpected);
        }
    }
}