using Squrl.App.Common;
using Squrl.App.Features.Items.DTOs;

namespace Squrl.App.Services.Inventory;

public interface IInventoryService
{
    Task<Result<GetManyItemsDto>> GetManyItemsAsync(string? query = null, int? pageNumber = null, int? pageSize = null, CancellationToken cancellationToken = default);
    Task<Result<GetItemDto>> CreateItemAsync(CreateItemDto item, CancellationToken cancellationToken = default);
    Task<Result<GetItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GetItemDto>> UpdateItemAsync(Guid id, UpdateItemDto item, CancellationToken cancellationToken = default);
    Task<Result<GetItemDto>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GetItemDto>> AddStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default);
    Task<Result<GetItemDto>> RemoveStocksAsync(Guid itemId, decimal quantity, CancellationToken cancellationToken = default);
    Task<Result<GetItemDto>> DynamicStockUpdateAsync(Guid itemId, string quantity, CancellationToken cancellationToken = default);
}