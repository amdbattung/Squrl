using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.Items.Queries;
using Squrl.App.Features.PurchaseOrderDetails.Queries;
using Squrl.App.Features.PurchaseOrders.Commands;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Features.PurchaseOrders.Mapping;
using Squrl.App.Features.PurchaseOrders.Queries;
using Squrl.App.Models;

namespace Squrl.App.Services.PurchaseOrder;

public partial class PurchaseOrderService
{
    public async Task<Result<GetPurchaseOrderDto>> ReceivePurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _mediator.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            
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

            var poDetailsResult = await _mediator
                .Send(new GetManyPoDetailsQuery(PurchaseOrderId: id, PageSize: null), cancellationToken);
            
            var result = await _transactionManager.ExecuteAsync(async ct =>
            {
                foreach (PurchaseOrderDetail poDetail in poDetailsResult.Value)
                {
                    Result<GetItemDto> addStocksResult = await _inventoryService
                        .AddStocksAsync(poDetail.Item.Id, poDetail.Quantity, ct);

                    if (!addStocksResult.IsSuccess)
                    {
                        return null;
                    }
                }

                UpdatePurchaseOrderDto updatePurchaseOrder = new()
                {
                    Status = PurchaseOrderStatus.Received,
                    DateShipped = _clock.GetCurrentInstant()
                };
                
                Models.PurchaseOrder? updatePurchaseOrderResult = await _mediator
                    .Send(new UpdatePurchaseOrderCommand(id, updatePurchaseOrder), ct);

                if (updatePurchaseOrderResult is null)
                {
                    return null;
                }

                return updatePurchaseOrderResult;
            }, cancellationToken);
                
            if (result == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Failed to receive purchase order.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to receive purchase order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _mediator.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            
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
                return Result<GetPurchaseOrderDto>.Fail("Purchase order cannot be cancelled.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            UpdatePurchaseOrderDto updatePurchaseOrder = new()
            {
                Status = PurchaseOrderStatus.Cancelled
            };
                
            Models.PurchaseOrder? result = await _mediator
                .Send(new UpdatePurchaseOrderCommand(id, updatePurchaseOrder), cancellationToken);
            
            if (result == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Failed to cancel purchase order.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to cancel purchase order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
    
    public async Task<Result<GetPurchaseOrderDto>> ReturnPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _mediator.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            
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

            var poDetailsResult = await _mediator
                .Send(new GetManyPoDetailsQuery(PurchaseOrderId: id, PageSize: null), cancellationToken);

            var failure = Result<GetPurchaseOrderDto>.Fail("Failed to return purchase order.")
                .WithFailureType(FailureType.BusinessLogic);
            
            Models.PurchaseOrder? result = await _transactionManager.ExecuteAsync(async ct =>
            {
                foreach (PurchaseOrderDetail poDetail in poDetailsResult.Value)
                {
                    Item? item = await _mediator
                        .Send(new GetItemByIdQuery(poDetail.Item.Id), ct);

                    if (item is null)
                    {
                        return null;
                    }

                    if (item.Quantity < poDetail.Quantity)
                    {
                        failure.WithMessage($"Not enough quantity to remove from item {item.Name}.");
                        return null;
                    }
                    
                    Result<GetItemDto> removeStocksResult = await _inventoryService
                        .RemoveStocksAsync(poDetail.Item.Id, poDetail.Quantity, ct);

                    if (!removeStocksResult.IsSuccess)
                    {
                        return null;
                    }
                }

                UpdatePurchaseOrderDto updatePurchaseOrder = new()
                {
                    Status = PurchaseOrderStatus.Returned
                };
                
                Models.PurchaseOrder? updatePurchaseOrderResult = await _mediator
                    .Send(new UpdatePurchaseOrderCommand(id, updatePurchaseOrder), ct);

                if (updatePurchaseOrderResult is null)
                {
                    return null;
                }

                return updatePurchaseOrderResult;
            }, cancellationToken);
                
            if (result == null)
            {
                return failure;
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to return purchase order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderStatusAsync(Guid id, PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        // NOT IMPLEMENTED
        return Task.FromResult<Result<GetPurchaseOrderDto>>(Result<GetPurchaseOrderDto>
            .Fail("Failed to update purchase order status.")
            .WithFailureType(FailureType.BusinessLogic));
    }

    public async Task<Result<GetPurchaseOrderDto>> RevertReceivedPurchaseOrderToPendingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _mediator.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            
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

            var poDetailsResult = await _mediator
                .Send(new GetManyPoDetailsQuery(PurchaseOrderId: id, PageSize: null), cancellationToken);

            var failure = Result<GetPurchaseOrderDto>.Fail("Failed to revert purchase order to pending.")
                .WithFailureType(FailureType.BusinessLogic);
            
            Models.PurchaseOrder? result = await _transactionManager.ExecuteAsync(async ct =>
            {
                foreach (PurchaseOrderDetail poDetail in poDetailsResult.Value)
                {
                    Item? item = await _mediator
                        .Send(new GetItemByIdQuery(poDetail.Item.Id), ct);

                    if (item is null)
                    {
                        return null;
                    }

                    if (item.Quantity < poDetail.Quantity)
                    {
                        failure.WithMessage($"Not enough quantity to remove from item {item.Name}.");
                        return null;
                    }
                    
                    Result<GetItemDto> removeStocksResult = await _inventoryService
                        .RemoveStocksAsync(poDetail.Item.Id, poDetail.Quantity, ct);

                    if (!removeStocksResult.IsSuccess)
                    {
                        return null;
                    }
                }

                UpdatePurchaseOrderDto updatePurchaseOrder = new()
                {
                    Status = PurchaseOrderStatus.Pending
                };
                
                Models.PurchaseOrder? updatePurchaseOrderResult = await _mediator
                    .Send(new UpdatePurchaseOrderCommand(id, updatePurchaseOrder), ct);

                if (updatePurchaseOrderResult is null)
                {
                    return null;
                }

                return updatePurchaseOrderResult;
            }, cancellationToken);
                
            if (result == null)
            {
                return failure;
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to revert purchase order to pending.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }

    public async Task<Result<GetPurchaseOrderDto>> UncancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            Models.PurchaseOrder? existingPurchaseOrder = await _mediator.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            
            if (existingPurchaseOrder == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order not found.")
                    .WithFailureType(FailureType.NotFound);
            }
            
            if (existingPurchaseOrder.Status != PurchaseOrderStatus.Cancelled)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order cannot be uncancelled.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            UpdatePurchaseOrderDto updatePurchaseOrder = new()
            {
                Status = PurchaseOrderStatus.Pending
            };
                
            Models.PurchaseOrder? result = await _mediator
                .Send(new UpdatePurchaseOrderCommand(id, updatePurchaseOrder), cancellationToken);
            
            if (result == null)
            {
                return Result<GetPurchaseOrderDto>.Fail("Failed to uncancel purchase order.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            return Result<GetPurchaseOrderDto>.Ok(PurchaseOrderMapper.ToDto(result));
        }
        catch (Exception e)
        {
            return Result<GetPurchaseOrderDto>.Fail("Failed to uncancel purchase order.")
                .WithException(e)
                .WithFailureType(FailureType.Exception);
        }
    }
}