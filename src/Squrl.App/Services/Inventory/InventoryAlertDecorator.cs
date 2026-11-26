using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Infrastructure.DecimalFormatter;
using Squrl.App.Services.Alert;

namespace Squrl.App.Services.Inventory;

public sealed class InventoryAlertDecorator : IInventoryService
{
    private readonly IInventoryService _inventoryService;
    private readonly IAlertService _alertService;
    private readonly IDecimalFormatter _decimalFormatter;

    public InventoryAlertDecorator(
        IInventoryService inventoryService,
        IAlertService alertService,
        IDecimalFormatter decimalFormatter)
    {
        _inventoryService = inventoryService;
        _alertService = alertService;
        _decimalFormatter = decimalFormatter;
    }
    
    public Task<Result<GetManyItemsDto>> GetManyItemsAsync(
        string? search = null,
        int pageNumber = 1,
        int pageSize = 10,
        SortDirection orderDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        return _inventoryService.GetManyItemsAsync(
            search,
            pageNumber,
            pageSize,
            orderDirection,
            cancellationToken);
    }

    public async Task<Result<GetItemDto>> CreateItemAsync(CreateItemDto item, Stream? photo = null, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.CreateItemAsync(item, photo, cancellationToken);

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

    public async Task<Result<GetItemDto>> UpdateItemAsync(Guid id, UpdateItemDto item, Stream? photo = null, CancellationToken cancellationToken = default)
    {
        Result<GetItemDto> result = await _inventoryService.UpdateItemAsync(id, item, photo, cancellationToken);

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

    public Task<Result> ExportItemsAsCsvAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        return _inventoryService.ExportItemsAsCsvAsync(destination, cancellationToken);
    }

    public Task<Result> ExportItemsAsExcelAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        return _inventoryService.ExportItemsAsExcelAsync(destination, cancellationToken);
    }

    private async Task AlertHelper(GetItemDto item, CancellationToken cancellationToken)
    {
        if (item.Quantity <= item.LowQuantityAlertThreshold)
        {
            await _alertService.SetAlertAsync(item.Id ?? Guid.Empty,
                $"Low stock warning for item {item.Name}. " +
                $"Current stock of {_decimalFormatter.FormatQuantity(item.Quantity)} " +
                $"is on or below the threshold of {_decimalFormatter
                    .RemoveTrailingZeros(item.LowQuantityAlertThreshold)}.",
                cancellationToken);
        }
        else
        {
            await _alertService.ClearAlertAsync(item.Id ?? Guid.Empty, cancellationToken);
        }
    }
}