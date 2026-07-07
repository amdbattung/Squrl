using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
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
        
        int totalCount = await query.CountAsync(cancellationToken);
        
        IReadOnlyList<Supplier> existingSuppliers = await query
            .OrderBy(s => EF.Property<Instant>(s, "DateCreated"))
            .ThenBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (existingSuppliers, pageNumber, pageSize, totalCount);
    }
}