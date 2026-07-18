using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrderDetails.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Handlers;

public class GetManyPoDetailsHandler : IRequestHandler<GetManyPoDetailsQuery, (IReadOnlyList<PurchaseOrderDetail> Value, int PageNumber, int PageSize, int TotalCount)>
{
    private readonly DataContext _dataContext;

    public GetManyPoDetailsHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<(IReadOnlyList<PurchaseOrderDetail> Value, int PageNumber, int PageSize, int TotalCount)> Handle(GetManyPoDetailsQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber ?? 1;
        int pageSize = request.PageSize ?? 10;

        IQueryable<PurchaseOrderDetail> query = _dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item);

        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<PurchaseOrderDetail> existingPoDetails = await query
            .OrderBy(p => EF.Property<Instant>(p, "DateCreated"))
            .ThenBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (existingPoDetails, pageNumber, pageSize, totalCount);
    }
}