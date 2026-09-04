using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Handlers;

public class GetManyPoDetailsHandler : IRequestHandler<GetManyPoDetailsQuery, (IReadOnlyList<PurchaseOrderDetail> Value, int? PageNumber, int? PageSize, int TotalCount)>
{
    private readonly DataContext _dataContext;

    public GetManyPoDetailsHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<(IReadOnlyList<PurchaseOrderDetail> Value, int? PageNumber, int? PageSize, int TotalCount)> Handle(GetManyPoDetailsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<PurchaseOrderDetail> query = _dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item);
        
        if (request.PurchaseOrderId.HasValue)
        {
            query = query.Where(p => p.PurchaseOrder.Id == request.PurchaseOrderId);

            query = request.OrderDirection == SortDirection.Descending
                ? query
                    .OrderByDescending(p => p.LineSequence)
                    .ThenByDescending(p => EF.Property<Instant>(p, "DateCreated"))
                    .ThenBy(p => p.Id)
                : query
                    .OrderBy(p => p.LineSequence)
                    .ThenBy(p => EF.Property<Instant>(p, "DateCreated"))
                    .ThenBy(p => p.Id);
        }
        else
        {
            query = request.OrderDirection == SortDirection.Descending
                ? query
                    .OrderByDescending(p => EF.Property<Instant>(p, "DateCreated"))
                    .ThenBy(p => p.Id)
                : query
                    .OrderBy(p => EF.Property<Instant>(p, "DateCreated"))
                    .ThenBy(p => p.Id);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<PurchaseOrderDetail> existingPoDetails;
        
        if (request.PageSize is null)
        {
            existingPoDetails = await query
                .ToListAsync(cancellationToken);
        }
        else
        {
            int pageNumber = request.PageNumber ?? 1;
            int pageSize = request.PageSize ?? 10;

            existingPoDetails = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        return (existingPoDetails,
            request.PageNumber ?? 1,
            request.PageSize ?? 0,
            totalCount);
    }
}