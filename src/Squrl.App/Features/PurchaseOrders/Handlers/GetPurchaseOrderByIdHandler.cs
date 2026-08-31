using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrders.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Handlers;

public class GetPurchaseOrderByIdHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrder?>
{
    private readonly DataContext _dataContext;

    public GetPurchaseOrderByIdHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PurchaseOrder?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        PurchaseOrder? existingPurchaseOrder = await _dataContext.PurchaseOrders
            .AsNoTracking()
            .Include(p => p.Supplier)
            .Where(i => i.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return existingPurchaseOrder;
    }
}