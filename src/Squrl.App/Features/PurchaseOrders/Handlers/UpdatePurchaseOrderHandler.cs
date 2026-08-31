using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrders.Commands;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Handlers;

public class UpdatePurchaseOrderHandler : IRequestHandler<UpdatePurchaseOrderCommand, PurchaseOrder?>
{
    private readonly DataContext _dataContext;

    public UpdatePurchaseOrderHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PurchaseOrder?> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if (existingPurchaseOrder == null)
        {
            return null;
        }

        UpdatePurchaseOrderDto requestPurchaseOrder = request.PurchaseOrder;
        
        existingPurchaseOrder.Supplier = requestPurchaseOrder.SupplierId == null
            ? existingPurchaseOrder.Supplier
            : await _dataContext.Suppliers.FirstOrDefaultAsync(s => s.Id == requestPurchaseOrder.SupplierId, cancellationToken) ?? existingPurchaseOrder.Supplier;
        existingPurchaseOrder.Status = requestPurchaseOrder.Status ?? existingPurchaseOrder.Status;
        existingPurchaseOrder.DateOrdered = requestPurchaseOrder.DateOrdered ?? existingPurchaseOrder.DateOrdered;
        existingPurchaseOrder.DateRequired = requestPurchaseOrder.DateRequired ?? existingPurchaseOrder.DateRequired;
        existingPurchaseOrder.DateShipped = requestPurchaseOrder.DateShipped ?? existingPurchaseOrder.DateShipped;
        
        await _dataContext.SaveChangesAsync(cancellationToken);
        return existingPurchaseOrder;
    }
}