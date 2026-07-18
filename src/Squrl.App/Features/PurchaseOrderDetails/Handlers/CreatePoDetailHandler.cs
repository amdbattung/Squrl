using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrderDetails.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Handlers;

public class CreatePoDetailHandler : IRequestHandler<CreatePoDetailCommand, PurchaseOrderDetail?>
{
    private readonly DataContext _dataContext;

    public CreatePoDetailHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PurchaseOrderDetail?> Handle(CreatePoDetailCommand request, CancellationToken cancellationToken)
    {
        Task<PurchaseOrder?> purchaseOrderTask =
            _dataContext.PurchaseOrders.FirstOrDefaultAsync(
                p => p.Id == request.PoDetail.PurchaseOrderId,
                cancellationToken);

        Task<Item?> itemTask =
            _dataContext.Items.FirstOrDefaultAsync(
                i => i.Id == request.PoDetail.ItemId,
                cancellationToken);

        await Task.WhenAll(purchaseOrderTask, itemTask);

        PurchaseOrder? purchaseOrder = purchaseOrderTask.Result;
        Item? item = itemTask.Result;
        
        if (purchaseOrder == null || item == null)
        {
            return null;
        }

        PurchaseOrderDetail newPoDetail = new()
        {
            Id = Guid.NewGuid(),
            PurchaseOrder = purchaseOrder,
            Item = item,
            Quantity = request.PoDetail.Quantity ?? 0m
        };
        
        await _dataContext.PurchaseOrderDetails.AddAsync(newPoDetail, cancellationToken);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? newPoDetail : null;
    }
}