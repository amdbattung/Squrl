using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Enums;
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
        IQueryable<PurchaseOrder> query = _dataContext.PurchaseOrders
            .AsNoTracking()
            .Include(p => p.Supplier);
        
        if (request.SupplierId.HasValue)
        {
            query = query.Where(p => p.Supplier != null && p.Supplier.Id == request.SupplierId);
        }

        List<PurchaseOrderStatus> statuses = new();

        if (request.IsPending)
        {
            statuses.AddRange([
                PurchaseOrderStatus.Pending,
                PurchaseOrderStatus.ToOrder,
                PurchaseOrderStatus.Ordered,
                PurchaseOrderStatus.InTransit
            ]);
        }

        if (request.IsReceived)
        {
            statuses.Add(PurchaseOrderStatus.Received);
        }

        if (request.IsCancelled)
        {
            statuses.Add(PurchaseOrderStatus.Cancelled);
        }

        if (request.HasFailed)
        {
            statuses.AddRange([
                PurchaseOrderStatus.Failed,
                PurchaseOrderStatus.Lost
            ]);
        }

        if (request.IsReturned)
        {
            statuses.Add(PurchaseOrderStatus.Returned);
        }

        if (statuses.Any())
        {
            query = query.Where(p => statuses.Contains(p.Status));
        }

        if (request.OrderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(p => EF.Property<Instant>(p, "DateCreated"))
                .ThenBy(p => p.Id);
        }
        else
        {
            query = query
                .OrderBy(p => EF.Property<Instant>(p, "DateCreated"))
                .ThenBy(p => p.Id);
        }
        
        IReadOnlyList<PurchaseOrder> existingPurchaseOrders;
        int pageNumber;
        int pageSize;
        int totalCount;
        
        if (request.PageSize is null)
        {
            existingPurchaseOrders = await query
                .ToListAsync(cancellationToken);
            
            pageNumber = 1;
            pageSize = existingPurchaseOrders.Count;
            totalCount = existingPurchaseOrders.Count;
        }
        else
        {
            pageNumber = request.PageNumber ?? 1;
            pageSize = request.PageSize ?? 10;
            totalCount = await query.CountAsync(cancellationToken);

            existingPurchaseOrders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        return (existingPurchaseOrders,
            pageNumber,
            pageSize,
            totalCount);
    }
}