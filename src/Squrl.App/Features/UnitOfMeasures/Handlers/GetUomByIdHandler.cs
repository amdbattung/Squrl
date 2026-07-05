using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.UnitOfMeasures.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Handlers;

public class GetUomByIdHandler : IRequestHandler<GetUomByIdQuery, UnitOfMeasure?>
{
    private readonly DataContext _dataContext;

    public GetUomByIdHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<UnitOfMeasure?> Handle(GetUomByIdQuery request, CancellationToken cancellationToken)
    {
        UnitOfMeasure? existingUom = await _dataContext.UnitOfMeasures
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return existingUom;
    }
}