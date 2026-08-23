using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.Items.DTOs;
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
                .Send(new GetManyPoDetailsQuery(PurchaseOrderId: id, PageSize: 500), cancellationToken);
            
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
                    Status = PurchaseOrderStatus.Received
                };
                
                Models.PurchaseOrder? updatePurchaseOrderResult = await _mediator
                    .Send(new UpdatePurchaseOrderCommand(id, updatePurchaseOrder), ct);

                if (updatePurchaseOrderResult is null)
                {
                    return null;
                }

                return existingPurchaseOrder;
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

    public Task<Result<GetPurchaseOrderDto>> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<GetPurchaseOrderDto>> UpdatePurchaseOrderStatusAsync(Guid id, PurchaseOrderStatus status, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<GetPurchaseOrderDto>> RevertReceivedPurchaseOrderToPendingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<GetPurchaseOrderDto>> UncancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}