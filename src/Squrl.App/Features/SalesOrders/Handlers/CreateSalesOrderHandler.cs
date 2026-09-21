using MediatR;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Features.SalesOrders.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrders.Handlers;

public class CreateSalesOrderHandler : IRequestHandler<CreateSalesOrderCommand, SalesOrder?>
{
    private readonly DataContext _dataContext;
    private readonly IClock _clock;

    public CreateSalesOrderHandler(DataContext dataContext,
        IClock clock)
    {
        _dataContext = dataContext;
        _clock = clock;
    }


    public async Task<SalesOrder?> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
    {
        SalesOrder newSalesOrder = new()
        {
            Id = Guid.NewGuid(),
            Customer = request.SalesOrder.Customer,
            InvoiceNumber = request.SalesOrder.InvoiceNumber,
            DateOrdered = request.SalesOrder.DateOrdered ?? _clock.GetCurrentInstant(),
        };
        
        await _dataContext.SalesOrders.AddAsync(newSalesOrder, cancellationToken);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? newSalesOrder : null;
    }
}