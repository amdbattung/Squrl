using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.SalesOrders.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Handlers;

public class GetManySalesOrdersHandler : IRequestHandler<GetManySalesOrdersQuery, (IReadOnlyList<SalesOrder> Value, int PageNumber, int PageSize, int TotalCount)>
{
    private readonly DataContext _dataContext;

    public GetManySalesOrdersHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<(IReadOnlyList<SalesOrder> Value, int PageNumber, int PageSize, int TotalCount)> Handle(GetManySalesOrdersQuery request, CancellationToken cancellationToken)
    {
        IQueryable<SalesOrder> query = _dataContext.SalesOrders
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(request.Customer))
        {
            query = query.Where(s =>
                EF.Functions.Like(s.Customer, $"%{request.Customer.Trim()}%"));
        }
        
        if (request.OrderDirection == SortDirection.Descending)
        {
            query = query
                .OrderByDescending(s => EF.Property<Instant>(s, "DateCreated"))
                .ThenBy(s => s.Id);
        }
        else
        {
            query = query
                .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
                .ThenBy(s => s.Id);
        }
        
        IReadOnlyList<SalesOrder> existingSalesOrders;
        int pageNumber;
        int pageSize;
        int totalCount;
        
        if (request.PageSize is null)
        {
            existingSalesOrders = await query
                .ToListAsync(cancellationToken);
            
            pageNumber = 1;
            pageSize = existingSalesOrders.Count;
            totalCount = existingSalesOrders.Count;
        }
        else
        {
            pageNumber = request.PageNumber ?? 1;
            pageSize = request.PageSize ?? 10;
            totalCount = await query.CountAsync(cancellationToken);

            existingSalesOrders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        return (existingSalesOrders,
            pageNumber,
            pageSize,
            totalCount);
    }
}