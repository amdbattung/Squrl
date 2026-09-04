using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Features.UnitOfMeasures.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Handlers;

public class GetManyUomsHandler : IRequestHandler<GetManyUomsQuery, (IReadOnlyList<UnitOfMeasure> Value, int PageNumber, int PageSize, int TotalCount)>
{
    private readonly DataContext _dataContext;

    public GetManyUomsHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<(IReadOnlyList<UnitOfMeasure> Value, int PageNumber, int PageSize, int TotalCount)> Handle(GetManyUomsQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber ?? 1;
        int pageSize = request.PageSize ?? 10;
        
        IQueryable<UnitOfMeasure> query = _dataContext.UnitOfMeasures
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            string pattern = $"%{request.Query.Trim()}%";

            query = query.Where(u =>
                EF.Functions.Like(u.Name, pattern) ||
                EF.Functions.Like(u.Code, pattern));
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
        
        IReadOnlyList<UnitOfMeasure> existingUoms = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (existingUoms, pageNumber, pageSize, totalCount);
    }
}