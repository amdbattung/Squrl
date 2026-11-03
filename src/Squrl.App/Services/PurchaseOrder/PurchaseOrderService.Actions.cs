using Microsoft.EntityFrameworkCore;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Features.PurchaseOrders.Mapping;
using Squrl.App.Models;

namespace Squrl.App.Services.PurchaseOrder;

public partial class PurchaseOrderService
{
    public async Task<Result<GetPurchaseOrderDto>> ReceivePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPurchaseOrderDto>>> action = async ct =>
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
                .Include(p => p.Supplier)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(ct);
        
            if (existingPurchaseOrder == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
        
            if (existingPurchaseOrder.Status != PurchaseOrderStatus.Pending &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.ToOrder &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.Ordered &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.InTransit &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.Lost &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.Failed)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order cannot be received.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            IReadOnlyList<PurchaseOrderDetail> existingPoDetails = await _dataContext.PurchaseOrderDetails
                .AsNoTracking()
                .Include(p => p.Item)
                .Where(p => p.PurchaseOrder.Id == id)
                .OrderBy(p => p.LineSequence)
                .ToListAsync(ct);
            
            foreach (PurchaseOrderDetail existingPoDetail in existingPoDetails)
            {
                Result<GetItemDto> addStocksResult = await _inventoryService
                    .AddStocksAsync(existingPoDetail.Item.Id, existingPoDetail.Quantity, ct);

                if (!addStocksResult.IsSuccess)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail($"Failed to add stocks in PO detail {existingPoDetail.LineSequence}; " +
                              $"{addStocksResult.Message}.")
                        .WithException(addStocksResult.Exception)
                        .WithFailureType(addStocksResult.FailureType);
                }
            }
            
            existingPurchaseOrder.Status = PurchaseOrderStatus.Received;
            existingPurchaseOrder.DateShipped = _clock.GetCurrentInstant();
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(existingPurchaseOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetPurchaseOrderDto>> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPurchaseOrderDto>>> action = async ct =>
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
                .Include(p => p.Supplier)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(ct);
        
            if (existingPurchaseOrder == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            if (existingPurchaseOrder.Status != PurchaseOrderStatus.Pending &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.ToOrder &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.Ordered &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.InTransit &&
                existingPurchaseOrder.Status != PurchaseOrderStatus.Lost)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order cannot be canceled.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            existingPurchaseOrder.Status = PurchaseOrderStatus.Canceled;
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(existingPurchaseOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
    
    public async Task<Result<GetPurchaseOrderDto>> ReturnPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPurchaseOrderDto>>> action = async ct =>
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
                .Include(p => p.Supplier)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(ct);
        
            if (existingPurchaseOrder == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            if (existingPurchaseOrder.Status != PurchaseOrderStatus.Received)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order cannot be returned.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            IReadOnlyList<PurchaseOrderDetail> existingPoDetails = await _dataContext.PurchaseOrderDetails
                .AsNoTracking()
                .Include(p => p.Item)
                .Where(p => p.PurchaseOrder.Id == id)
                .OrderBy(p => p.LineSequence)
                .ToListAsync(ct);

            foreach (PurchaseOrderDetail existingPoDetail in existingPoDetails)
            {
                Item? item = await _dataContext.Items
                    .AsNoTracking()
                    .Where(i => i.Id == existingPoDetail.Item.Id)
                    .FirstOrDefaultAsync(ct);
                
                if (item is null)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail($"Failed to remove stocks in PO detail {existingPoDetail.LineSequence}; " +
                              $"Item not found.")
                        .WithFailureType(FailureType.NotFound);
                }

                if (item.Quantity < existingPoDetail.Quantity)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail($"Failed to remove stocks in PO detail {existingPoDetail.LineSequence}; " +
                              $"Not enough stocks to remove from item {item.Name}.")
                        .WithFailureType(FailureType.BusinessLogic);
                }
                
                Result<GetItemDto> removeStocksResult = await _inventoryService
                    .RemoveStocksAsync(existingPoDetail.Item.Id, existingPoDetail.Quantity, ct);
                
                if (!removeStocksResult.IsSuccess)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail($"Failed to remove stocks in PO detail {existingPoDetail.LineSequence}; " +
                              $"{removeStocksResult.Message}.")
                        .WithException(removeStocksResult.Exception)
                        .WithFailureType(removeStocksResult.FailureType);
                }
            }
            
            existingPurchaseOrder.Status = PurchaseOrderStatus.Returned;
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(existingPurchaseOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderStatusAsync(Guid id, PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        // NOT IMPLEMENTED
        return Task.FromResult<Result<GetPurchaseOrderDto>>(Result<GetPurchaseOrderDto>
            .Fail("Failed to update purchase order status.")
            .WithFailureType(FailureType.Unexpected));
    }

    public async Task<Result<GetPurchaseOrderDto>> RevertReceivedPurchaseOrderToPendingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPurchaseOrderDto>>> action = async ct =>
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
                .Include(p => p.Supplier)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(ct);
        
            if (existingPurchaseOrder == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            if (existingPurchaseOrder.Status != PurchaseOrderStatus.Received)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order cannot be reverted to pending.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            IReadOnlyList<PurchaseOrderDetail> existingPoDetails = await _dataContext.PurchaseOrderDetails
                .AsNoTracking()
                .Include(p => p.Item)
                .Where(p => p.PurchaseOrder.Id == id)
                .OrderBy(p => p.LineSequence)
                .ToListAsync(ct);

            foreach (PurchaseOrderDetail existingPoDetail in existingPoDetails)
            {
                Item? item = await _dataContext.Items
                    .AsNoTracking()
                    .Where(i => i.Id == existingPoDetail.Item.Id)
                    .FirstOrDefaultAsync(ct);
                
                if (item is null)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail($"Failed to remove stocks in PO detail {existingPoDetail.LineSequence}; " +
                              $"Item not found.")
                        .WithFailureType(FailureType.NotFound);
                }

                if (item.Quantity < existingPoDetail.Quantity)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail($"Failed to remove stocks in PO detail {existingPoDetail.LineSequence}; " +
                              $"Not enough stocks to remove from item {item.Name}.")
                        .WithFailureType(FailureType.BusinessLogic);
                }
                
                Result<GetItemDto> removeStocksResult = await _inventoryService
                    .RemoveStocksAsync(existingPoDetail.Item.Id, existingPoDetail.Quantity, ct);
                
                if (!removeStocksResult.IsSuccess)
                {
                    return Result<GetPurchaseOrderDto>
                        .Fail($"Failed to remove stocks in PO detail {existingPoDetail.LineSequence}; " +
                              $"{removeStocksResult.Message}.")
                        .WithException(removeStocksResult.Exception)
                        .WithFailureType(removeStocksResult.FailureType);
                }
            }
            
            existingPurchaseOrder.Status = PurchaseOrderStatus.Pending;
            existingPurchaseOrder.DateShipped = null;
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(existingPurchaseOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }

    public async Task<Result<GetPurchaseOrderDto>> UncancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Func<CancellationToken, Task<Result<GetPurchaseOrderDto>>> action = async ct =>
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
                .Include(p => p.Supplier)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(ct);
            
            if (existingPurchaseOrder == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            if (existingPurchaseOrder.Status != PurchaseOrderStatus.Canceled)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order cannot be uncanceled.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            existingPurchaseOrder.Status = PurchaseOrderStatus.Pending;
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(existingPurchaseOrder));
        };
        
        return await _transactionManager.ExecuteAsync(action, cancellationToken);
    }
}