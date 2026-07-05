using Squrl.App.Common;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Services.Alert;

namespace Squrl.App.Services.Inventory;

public class InventoryAlertDecorator : IInventoryService
{
    private readonly IInventoryService _inventoryService;
    private readonly IAlertService _alertService;
    
    public InventoryAlertDecorator(IInventoryService inventoryService,
        IAlertService alertService)
    {
        _inventoryService = inventoryService;
        _alertService = alertService;
    }
    
    public Task<Result<GetManyItemsDto>> GetManyItemsAsync(string? query = null, int? pageNumber = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        return _inventoryService.GetManyItemsAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<GetItemDto>> CreateItemAsync(CreateItemDto item, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.CreateItemAsync(item, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }
        
        if (result.Value is not null)
        {
            await AlertHelper(result.Value, cancellationToken);
        }
        
        return result;
    }

    public Task<Result<GetItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _inventoryService.GetItemByIdAsync(id, cancellationToken);
    }

    public async Task<Result<GetItemDto>> UpdateItemAsync(Guid id, UpdateItemDto item, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.UpdateItemAsync(id, item, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }
        
        if (result.Value is not null)
        {
            await AlertHelper(result.Value, cancellationToken);
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.DeleteItemAsync(id, cancellationToken);
        
        if (result.IsSuccess)
        {
            await _alertService.ClearAlertAsync(result.Value?.Id ?? Guid.Empty, cancellationToken);
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> AddStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.AddStocksAsync(itemId, quantity, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }
        
        if (result.Value is not null)
        {
            await AlertHelper(result.Value, cancellationToken);
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> RemoveStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.RemoveStocksAsync(itemId, quantity, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }

        if (result.Value is not null)
        {
            await AlertHelper(result.Value, cancellationToken);
        }
        
        return result;
    }

    public async Task<Result<GetItemDto>> DynamicStockUpdateAsync(Guid itemId, string quantity, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.DynamicStockUpdateAsync(itemId, quantity, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }

        if (result.Value is not null)
        {
            await AlertHelper(result.Value, cancellationToken);
        }
        
        return result;
    }

    private async Task AlertHelper(GetItemDto item, CancellationToken cancellationToken)
    {
        if (item.Quantity <= item.LowQuantityAlertThreshold)
        {
            await _alertService.SetAlertAsync(item.Id ?? Guid.Empty,
                $"Low stock warning for item {item.Name}. " +
                $"Current stock of {item.Quantity:#,##0.##} " +
                $"is on or below the threshold of {item.LowQuantityAlertThreshold:#,##0.##}.",
                cancellationToken);
        }
        else
        {
            await _alertService.ClearAlertAsync(item.Id ?? Guid.Empty, cancellationToken);
        }
    }
}