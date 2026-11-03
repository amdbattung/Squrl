using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;

namespace Squrl.App.Services.Inventory;

public class InventoryLoggingDecorator : IInventoryService
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryLoggingDecorator> _logger;

    public InventoryLoggingDecorator(
        IInventoryService inventoryService,
        ILogger<InventoryLoggingDecorator> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task<Result<GetManyItemsDto>> GetManyItemsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        Result<GetManyItemsDto> result;

        try
        {
            result = await _inventoryService.GetManyItemsAsync(
                search,
                pageNumber,
                pageSize,
                orderDirection,
                cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Many Items Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Many Items Request: {Count} items successfully fetched.",
                result.Value?.Items?.Count);
        }
        else
        {
            _logger.LogError(result.Exception,
                "Get Many Items Request: unhandled exception type {FailureType}.",
                nameof(result.FailureType));
            throw result.Exception
                  ?? new Exception(result.Message ?? "Failed to retrieve items, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> CreateItemAsync(
        CreateItemDto item,
        Stream? photo = null,
        CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result;

        try
        {
            result = await _inventoryService.CreateItemAsync(item, photo, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Create Item Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Create Item Request: item {Name} successfully created.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Create Item Request: failed to create item {Name}, invalid item.",
                    result.Value?.Name);
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception,
                    "Create Item Request: failed to create item.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Create Item Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create item, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Create Item Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to create item, something went wrong.");
        }
    
        return result;
    }

    public async Task<Result<GetItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result;

        try
        {
            result = await _inventoryService.GetItemByIdAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get Item By ID Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Get Item By ID Request: item {Name} successfully fetched.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Get Item By ID Request: failed to fetch, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Get Item By ID Request: failed to fetch, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Get Item By ID Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve item, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Get Item By ID Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to retrieve item, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> UpdateItemAsync(
        Guid id,
        UpdateItemDto item,
        Stream? photo = null,
        CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result;

        try
        {
            result = await _inventoryService.UpdateItemAsync(id, item, photo, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Item Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update Item Request: item {Name} successfully updated.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Update Item Request: failed to update, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Update Item Request: failed to update, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Update Item Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update item, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Update Item Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update item, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result;

        try
        {
            result = await _inventoryService.DeleteItemAsync(id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Delete Item Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Delete Item Request: item {Name} successfully deleted.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Delete Item Request: failed to delete, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Delete Item Request: failed to update, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Delete Item Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete item, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Delete Item Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to delete item, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> AddStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result;

        try
        {
            result = await _inventoryService.AddStocksAsync(itemId, quantity, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Add Stocks Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Add Stocks Request: {Quantity} stocks added to item {Name}.",
                result.Value?.Name,
                quantity);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Add Stocks Request: failed to add stocks, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Add Stocks Request: failed to add stocks, item not found.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Add Stocks Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to add stocks, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Add Stocks Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to add stocks, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> RemoveStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result;

        try
        {
            result = await _inventoryService.RemoveStocksAsync(itemId, quantity, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Remove Stocks Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Remove Stocks Request: {Quantity} stocks removed from item {Name}.",
                result.Value?.Name,
                quantity);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Remove Stocks Request: failed to remove stocks, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Remove Stocks Request: failed to remove stocks, item not found.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception,
                    "Create Item Request: failed to create item.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Remove Stocks Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to remove stocks, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Remove Stocks Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to remove stocks, something went wrong.");
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> DynamicStockUpdateAsync(Guid itemId, string quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result;

        try
        {
            result = await _inventoryService.DynamicStockUpdateAsync(itemId, quantity, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Update Stocks Request: unhandled exception.");
            throw;
        }

        if (result.IsSuccess)
        {
            _logger.LogInformation("Update Stocks Request: stocks updated from item {Name}.",
                result.Value?.Name);
        }
        else switch (result.FailureType)
        {
            case FailureType.Validation:
                _logger.LogInformation(result.Exception,
                    "Update Stocks Request: failed to update stocks, invalid request.");
                break;
            
            case FailureType.NotFound:
                _logger.LogInformation(result.Exception,
                    "Update Stocks Request: failed to update stocks, item not found.");
                break;
            
            case FailureType.BusinessLogic:
                _logger.LogError(result.Exception,
                    "Create Item Request: failed to create item.");
                break;
            
            case FailureType.Exception:
                _logger.LogError(result.Exception,
                    "Update Stocks Request: unhandled exception.");
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update stocks, something went wrong.");
            
            default:
                _logger.LogError(result.Exception,
                    "Update Stocks Request: unhandled exception type {FailureType}.",
                    nameof(result.FailureType));
                throw result.Exception
                      ?? new Exception(result.Message ?? "Failed to update stocks, something went wrong.");
        }
        
        return result;
    }
}