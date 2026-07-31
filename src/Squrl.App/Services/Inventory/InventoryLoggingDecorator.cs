using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;

namespace Squrl.App.Services.Inventory;

public class InventoryLoggingDecorator : IInventoryService
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryLoggingDecorator> _logger;

    public InventoryLoggingDecorator(IInventoryService inventoryService,
        ILogger<InventoryLoggingDecorator> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task<Result<GetManyItemsDto>> GetManyItemsAsync(string? query = null, int? pageNumber = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        Result<GetManyItemsDto> result = await _inventoryService.GetManyItemsAsync(query, pageNumber, pageSize, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Many Items Request: {result.Value?.Items?.Count} items successfully fetched.");
        }
        else if (result.FailureType == FailureType.Exception)
        {
            _logger.LogError(result.Exception, "Get Many Items Request: unhandled exception.");
            return Result<GetManyItemsDto>.Fail(result.Message ?? "Failed to retrieve items.")
                .WithFailureType(FailureType.Unexpected);
        }
        else
        {
            _logger.LogError(result.Exception, "Get Many Items Request: failed to fetch items.");
            return Result<GetManyItemsDto>.Fail(result.Message ?? "Failed to retrieve items.")
                .WithFailureType(FailureType.Unexpected);
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> CreateItemAsync(CreateItemDto item, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.CreateItemAsync(item, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Create Item Request: item {result.Value?.Name} successfully created.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation($"Create Item Request: failed to create item {result.Value?.Name}, invalid item.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Create Item Request: unhandled exception.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to create item.");
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception, "Create Item Request: failed to create item.");
                break;
            
            default:
                _logger.LogError(result.Exception, "Create Item Request: failed to create item.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to create item.");
        }
    
        return result;
    }

    public async Task<Result<GetItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.GetItemByIdAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Get Item By ID Request: item {result.Value?.Name} successfully fetched.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Get Item By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Get Item By ID Request: failed to fetch, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Get Item By ID Request: unhandled exception.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to retrieve item.");
            
            default:
                _logger.LogError(result.Exception, "Get Item By ID Request: failed to fetch item.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to retrieve item.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> UpdateItemAsync(Guid id, UpdateItemDto item, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.UpdateItemAsync(id, item, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update Item Request: item {result.Value?.Name} successfully updated.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Update Item Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Update Item Request: failed to update, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update Item Request: unhandled exception.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to update item.");
            
            default:
                _logger.LogError(result.Exception, "Update Item Request: failed to update item.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to update item.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.DeleteItemAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Delete Item Request: item {result.Value?.Name} successfully deleted.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Delete Item Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Delete Item Request: failed to update, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Delete Item Request: unhandled exception.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to delete item.");
            
            default:
                _logger.LogError(result.Exception, "Delete Item Request: failed to update item.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to delete item.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> AddStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.AddStocksAsync(itemId, quantity, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Add Stocks Request: stocks added to item {result.Value?.Name}," +
                                   $"{quantity} stocks added.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Add Stocks Request: failed to add stocks, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Add Stocks Request: failed to add stocks, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update Item Request: unhandled exception.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to add stocks.");
            
            default:
                _logger.LogError(result.Exception, "Add Stocks Request: failed to add stocks to item.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to add stocks.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> RemoveStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.RemoveStocksAsync(itemId, quantity, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Remove Stocks Request: stocks removed from item {result.Value?.Name}," +
                                   $"{quantity} stocks removed.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Remove Stocks Request: failed to remove stocks, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Remove Stocks Request: failed to remove stocks, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Remove Stocks Request: unhandled exception.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to remove stocks.");
            
            default:
                _logger.LogError(result.Exception, "Remove Stocks Request: failed to remove stocks from item.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to remove stocks.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> DynamicStockUpdateAsync(Guid itemId, string quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.DynamicStockUpdateAsync(itemId, quantity, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Update Stocks Request: stocks updated from item {result.Value?.Name}.");
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation("Update Stocks Request: failed to update stocks, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation("Update Stocks Request: failed to update stocks, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception, "Update Stocks Request: unhandled exception.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to update stocks.");
            
            default:
                _logger.LogError(result.Exception, "Update Stocks Request: failed to update stocks from item.");
                return Result<GetItemDto>.Fail(result.Message ?? "Failed to update stocks.");
        }
        
        return result;
    }
}