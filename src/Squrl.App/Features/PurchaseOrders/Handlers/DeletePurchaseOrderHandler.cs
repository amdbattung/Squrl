using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrders.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Handlers;

public class DeletePurchaseOrderHandler : IRequestHandler<DeletePurchaseOrderCommand, PurchaseOrder?>
{
    private readonly DataContext _dataContext;

    public DeletePurchaseOrderHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PurchaseOrder?> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if (existingPurchaseOrder == null)
        {
            return null;
        }
        
        _dataContext.PurchaseOrders.Remove(existingPurchaseOrder);
        
        return existingPurchaseOrder;
    }
}