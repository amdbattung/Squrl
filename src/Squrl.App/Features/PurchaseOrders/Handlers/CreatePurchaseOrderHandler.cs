using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrders.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Handlers;

public class CreatePurchaseOrderHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrder?>
{
    private readonly DataContext _dataContext;
    private readonly IClock _clock;

    public CreatePurchaseOrderHandler(DataContext dataContext,
    IClock clock)
    {
        _dataContext = dataContext;
        _clock = clock;
    }

    public async Task<PurchaseOrder?> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        Supplier? supplier = await _dataContext.Suppliers.FirstOrDefaultAsync(s => s.Id == request.PurchaseOrder.SupplierId, cancellationToken);
        
        if (supplier == null)
        {
            return null;
        }
        
        PurchaseOrder newPurchaseOrder = new()
        {
            Id = Guid.NewGuid(),
            Supplier = supplier,
            Status = request.PurchaseOrder.Status ?? default,
            DateOrdered = request.PurchaseOrder.DateOrdered ?? _clock.GetCurrentInstant(),
            DateRequired =  request.PurchaseOrder.DateRequired,
            DateShipped = request.PurchaseOrder.DateShipped
        };
        
        await _dataContext.PurchaseOrders.AddAsync(newPurchaseOrder, cancellationToken);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? newPurchaseOrder : null;
    }
}