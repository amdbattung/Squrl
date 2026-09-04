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
        int pageNumber = request.PageNumber ?? 1;
        int pageSize = request.PageSize ?? 10;

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

        int totalCount = await query.CountAsync(cancellationToken);

        IReadOnlyList<Item> existingItems = await query
            .OrderBy(i => EF.Property<Instant>(i, "DateCreated"))
            .ThenBy(i => i.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (existingItems, pageNumber, pageSize, totalCount);
    }
}