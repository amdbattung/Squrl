using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrderDetails.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Handlers;

public class GetManySoDetailsHandler : IRequestHandler<GetManySoDetailsQuery, (IReadOnlyList<SalesOrderDetail> Value, int PageNumber, int PageSize, int TotalCount)>
{
    private readonly DataContext _dataContext;

    public GetManySoDetailsHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<(IReadOnlyList<SalesOrderDetail> Value, int PageNumber, int PageSize, int TotalCount)> Handle(GetManySoDetailsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<SalesOrderDetail> query = _dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.SalesOrder)
            .Include(s => s.Item);
        
        if (request.SalesOrderId.HasValue)
        {
            query = query.Where(s => s.SalesOrder.Id == request.SalesOrderId);

            query = request.OrderDirection == SortDirection.Descending
                ? query
                    .OrderByDescending(s => s.LineSequence)
                    .ThenByDescending(s => EF.Property<Instant>(s, "DateCreated"))
                    .ThenBy(s => s.Id)
                : query
                    .OrderBy(s => s.LineSequence)
                    .ThenBy(s => EF.Property<Instant>(s, "DateCreated"))
                    .ThenBy(s => s.Id);
        }
        else
        {
            query = request.OrderDirection == SortDirection.Descending
                ? query
                    .OrderByDescending(s => EF.Property<Instant>(s, "DateCreated"))
                    .ThenBy(s => s.Id)
                : query
                    .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
                    .ThenBy(s => s.Id);
        }
        
        IReadOnlyList<SalesOrderDetail> existingSoDetails;
        int pageNumber;
        int pageSize;
        int totalCount;
        
        if (request.PageSize is null)
        {
            existingSoDetails = await query
                .ToListAsync(cancellationToken);
            
            pageNumber = 1;
            pageSize = existingSoDetails.Count;
            totalCount = existingSoDetails.Count;
        }
        else
        {
            pageNumber = request.PageNumber ?? 1;
            pageSize = request.PageSize ?? 10;
            totalCount = await query.CountAsync(cancellationToken);

            existingSoDetails = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        
        return (existingSoDetails,
            pageNumber,
            pageSize,
            totalCount);
    }
}