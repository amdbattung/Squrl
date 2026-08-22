using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrderDetails.Commands;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Handlers;

public class UpdatePoDetailHandler : IRequestHandler<UpdatePoDetailCommand, PurchaseOrderDetail?>
{
    private readonly DataContext _dataContext;

    public UpdatePoDetailHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PurchaseOrderDetail?> Handle(UpdatePoDetailCommand request, CancellationToken cancellationToken)
    {
        PurchaseOrderDetail? existingPoDetail = await _dataContext.PurchaseOrderDetails
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if (existingPoDetail == null)
        {
            return null;
        }

        UpdatePoDetailDto requestPoDetail = request.PoDetail;
        
        existingPoDetail.PurchaseOrder = requestPoDetail.PurchaseOrderId == null
            ? existingPoDetail.PurchaseOrder
            : await _dataContext.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == requestPoDetail.PurchaseOrderId, cancellationToken) ?? existingPoDetail.PurchaseOrder;
        existingPoDetail.Item = requestPoDetail.ItemId == null
            ? existingPoDetail.Item
            : await _dataContext.Items.FirstOrDefaultAsync(p => p.Id == requestPoDetail.ItemId, cancellationToken) ?? existingPoDetail.Item;
        existingPoDetail.Quantity = requestPoDetail.Quantity ?? existingPoDetail.Quantity;

        return existingPoDetail;
    }
}