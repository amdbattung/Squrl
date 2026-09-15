using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.SalesOrderDetails.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Handlers;

public class CreateSoDetailHandler : IRequestHandler<CreateSoDetailCommand, SalesOrderDetail?>
{
    private readonly DataContext _dataContext;

    public CreateSoDetailHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<SalesOrderDetail?> Handle(CreateSoDetailCommand request, CancellationToken cancellationToken)
    {
        SalesOrder? salesOrder = await _dataContext.SalesOrders
            .FirstOrDefaultAsync(
                s => s.Id == request.SoDetail.SalesOrderId,
                cancellationToken);
        
        Item? item = await _dataContext.Items
            .FirstOrDefaultAsync(
                i => i.Id == request.SoDetail.ItemId,
                cancellationToken);

        if (salesOrder == null || item == null)
        {
            return null;
        }

        SalesOrderDetail newSoDetail = new()
        {
            Id = Guid.NewGuid(),
            SalesOrder = salesOrder,
            LineSequence = request.SoDetail.LineSequence ?? 0,
            Item = item,
            UnitPrice = request.SoDetail.UnitPrice ?? 0m,
            Quantity = request.SoDetail.UnitPrice ?? 0m
        };
            
        await _dataContext.SalesOrderDetails.AddAsync(newSoDetail, cancellationToken);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? newSoDetail : null;
    }
}