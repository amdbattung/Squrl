using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.SalesOrders.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Handlers;

public class GetSalesOrderByIdHandler : IRequestHandler<GetSalesOrderByIdQuery, SalesOrder?>
{
    private readonly DataContext _dataContext;

    public GetSalesOrderByIdHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<SalesOrder?> Handle(GetSalesOrderByIdQuery request, CancellationToken cancellationToken)
    {
        SalesOrder? existingSalesOrder = await _dataContext.SalesOrders
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return existingSalesOrder;
    }
}