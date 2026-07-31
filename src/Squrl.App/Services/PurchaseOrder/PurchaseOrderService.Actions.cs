using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Features.PurchaseOrders.Queries;

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

            if (existingPurchaseOrder.Status != PurchaseOrderStatus.Pending ||
                existingPurchaseOrder.Status != PurchaseOrderStatus.ToOrder ||
                existingPurchaseOrder.Status != PurchaseOrderStatus.Ordered ||
                existingPurchaseOrder.Status != PurchaseOrderStatus.InTransit ||
                existingPurchaseOrder.Status != PurchaseOrderStatus.Lost ||
                existingPurchaseOrder.Status != PurchaseOrderStatus.Failed)
            {
                return Result<GetPurchaseOrderDto>.Fail("Purchase order cannot be received.")
                    .WithFailureType(FailureType.BusinessLogic);
            }
            
            // TODO
            
            // GET PO DETAILS
            
            // ITERATE THROUGH PO DETAILS
                // ADD TO STOCKS
                
            // UPDATE PO TO RECEIVED
                
            // RETURN PURCHASE ORDER
            
            throw new NotImplementedException();
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