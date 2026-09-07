using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Items.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Handlers;

public class GetManyItemsHandler : IRequestHandler<GetManyItemsQuery, (IReadOnlyList<Item> Value, int PageNumber, int PageSize, int TotalCount)>
{
    private readonly DataContext _dataContext;

    public GetManyItemsHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<(IReadOnlyList<Item> Value, int PageNumber, int PageSize, int TotalCount)> Handle(GetManyItemsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Item> query = _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom);
        
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            query = query.Where(i =>
                EF.Functions.Like(i.Name, $"%{request.Query.Trim()}%"));
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

        IReadOnlyList<Item> existingItems;
        int pageNumber;
        int pageSize;
        int totalCount;
        
        if (request.PageSize is null)
        {
            existingItems = await query
                .ToListAsync(cancellationToken);
            
            pageNumber = 1;
            pageSize = existingItems.Count;
            totalCount = existingItems.Count;
        }
        else
        {
            pageNumber = request.PageNumber ?? 1;
            pageSize = request.PageSize ?? 10;
            totalCount = await query.CountAsync(cancellationToken);

            existingItems = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        return (existingItems,
            pageNumber,
            pageSize,
            totalCount);
    }
}