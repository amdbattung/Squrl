using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrders.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Handlers;

public class GetManyPurchaseOrdersHandler : IRequestHandler<GetManyPurchaseOrdersQuery, (IReadOnlyList<PurchaseOrder> Value, int PageNumber, int PageSize, int TotalCount)>
{
    private readonly DataContext _dataContext;

    public GetManyPurchaseOrdersHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<(IReadOnlyList<PurchaseOrder> Value, int PageNumber, int PageSize, int TotalCount)> Handle(GetManyPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber ?? 1;
        int pageSize = request.PageSize ?? 10;

        IQueryable<PurchaseOrder> query = _dataContext.PurchaseOrders
            .AsNoTracking()
            .Include(p => p.Supplier);
        
        if (request.SupplierId.HasValue)
        {
            query = query.Where(p => p.Supplier != null && p.Supplier.Id == request.SupplierId);
        }
        
        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<PurchaseOrder> existingPurchaseOrders = await query
            .OrderBy(p => EF.Property<Instant>(p, "DateCreated"))
            .ThenBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (existingPurchaseOrders, pageNumber, pageSize, totalCount);
    }
}