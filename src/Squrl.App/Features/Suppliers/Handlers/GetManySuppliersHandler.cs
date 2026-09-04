using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.Suppliers.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.Suppliers.Handlers;

public class GetManySuppliersHandler : IRequestHandler<GetManySuppliersQuery, (IReadOnlyList<Supplier> Value, int PageNumber, int PageSize, int TotalCount)>
{
    private readonly DataContext _dataContext;

    public GetManySuppliersHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<(IReadOnlyList<Supplier> Value, int PageNumber, int PageSize, int TotalCount)> Handle(GetManySuppliersQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber ?? 1;
        int pageSize = request.PageSize ?? 10;
        
        IQueryable<Supplier> query = _dataContext.Suppliers
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            query = query.Where(s =>
                EF.Functions.Like(s.Name, $"%{request.Query.Trim()}%"));
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
        
        IReadOnlyList<Supplier> existingSuppliers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (existingSuppliers, pageNumber, pageSize, totalCount);
    }
}