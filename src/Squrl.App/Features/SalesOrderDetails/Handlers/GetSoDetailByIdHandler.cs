using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.SalesOrderDetails.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Handlers;

public class GetSoDetailByIdHandler : IRequestHandler<GetSoDetailByIdQuery, SalesOrderDetail?>
{
    private readonly DataContext _dataContext;

    public GetSoDetailByIdHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<SalesOrderDetail?> Handle(GetSoDetailByIdQuery request, CancellationToken cancellationToken)
    {
        SalesOrderDetail? existingSoDetail = await _dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.SalesOrder)
            .Include(s => s.Item)
            .Where(s => s.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return existingSoDetail;
    }
}