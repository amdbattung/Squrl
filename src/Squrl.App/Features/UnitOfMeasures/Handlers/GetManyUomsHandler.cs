using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
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
        
        int totalCount = await query.CountAsync(cancellationToken);
        
        IReadOnlyList<UnitOfMeasure> existingUoms = await query
            .OrderBy(u => EF.Property<Instant>(u, "DateCreated"))
            .ThenBy(u => u.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (existingUoms, pageNumber, pageSize, totalCount);
    }
}