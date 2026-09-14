using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.SalesOrders.Commands;
using Squrl.App.Features.SalesOrders.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Handlers;

public class UpdateSalesOrderHandler : IRequestHandler<UpdateSalesOrderCommand, SalesOrder?>
{
    private readonly DataContext _dataContext;

    public UpdateSalesOrderHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<SalesOrder?> Handle(UpdateSalesOrderCommand request, CancellationToken cancellationToken)
    {
        SalesOrder? existingSalesOrder = await _dataContext.SalesOrders
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        
        if (existingSalesOrder == null)
        {
            return null;
        }
        
        UpdateSalesOrderDto requestSalesOrder = request.SalesOrder;
        
        existingSalesOrder.Customer = requestSalesOrder.Customer;
        existingSalesOrder.InvoiceNumber = requestSalesOrder.InvoiceNumber ?? existingSalesOrder.InvoiceNumber;
        existingSalesOrder.DateOrdered = requestSalesOrder.DateOrdered ?? existingSalesOrder.DateOrdered;
        
        await _dataContext.SaveChangesAsync(cancellationToken);
        return existingSalesOrder;
    }
}