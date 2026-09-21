using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.SalesOrders.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Handlers;

public class DeleteSalesOrderHandler : IRequestHandler<DeleteSalesOrderCommand, SalesOrder?>
{
    private readonly DataContext _dataContext;

    public DeleteSalesOrderHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<SalesOrder?> Handle(DeleteSalesOrderCommand request, CancellationToken cancellationToken)
    {
        SalesOrder? existingSalesOrder = await _dataContext.SalesOrders
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        
        if (existingSalesOrder == null)
        {
            return null;
        }
        
        _dataContext.SalesOrders.Remove(existingSalesOrder);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? existingSalesOrder : null;
    }
}