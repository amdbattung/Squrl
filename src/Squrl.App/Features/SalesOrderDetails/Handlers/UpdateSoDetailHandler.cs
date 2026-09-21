using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.SalesOrderDetails.Commands;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Handlers;

public class UpdateSoDetailHandler : IRequestHandler<UpdateSoDetailCommand, SalesOrderDetail?>
{
    private readonly DataContext _dataContext;

    public UpdateSoDetailHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<SalesOrderDetail?> Handle(UpdateSoDetailCommand request, CancellationToken cancellationToken)
    {
        SalesOrderDetail? existingSoDetail = await _dataContext.SalesOrderDetails
            .Include(s => s.SalesOrder)
            .Include(s => s.Item)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        
        if (existingSoDetail == null)
        {
            return null;
        }

        UpdateSoDetailDto requestSoDetail = request.SoDetail;
        
        existingSoDetail.SalesOrder = requestSoDetail.SalesOrderId == null
            ? existingSoDetail.SalesOrder
            : await _dataContext.SalesOrders.FirstOrDefaultAsync(s =>
                s.Id == requestSoDetail.SalesOrderId, cancellationToken) ?? existingSoDetail.SalesOrder;
        existingSoDetail.LineSequence = requestSoDetail.LineSequence ?? existingSoDetail.LineSequence;
        existingSoDetail.Item = requestSoDetail.ItemId == null
            ? existingSoDetail.Item
            : await _dataContext.Items.FirstOrDefaultAsync(s =>
                s.Id == requestSoDetail.ItemId, cancellationToken) ?? existingSoDetail.Item;
        existingSoDetail.UnitPrice = requestSoDetail.UnitPrice ?? existingSoDetail.Quantity;
        existingSoDetail.Quantity = requestSoDetail.Quantity ?? existingSoDetail.Quantity;
        
        await _dataContext.SaveChangesAsync(cancellationToken);
        return existingSoDetail;
    }
}