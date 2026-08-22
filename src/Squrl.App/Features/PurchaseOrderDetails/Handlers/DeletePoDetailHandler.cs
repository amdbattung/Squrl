using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrderDetails.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Handlers;

public class DeletePoDetailHandler : IRequestHandler<DeletePoDetailCommand, PurchaseOrderDetail?>
{
    private readonly DataContext _dataContext;

    public DeletePoDetailHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PurchaseOrderDetail?> Handle(DeletePoDetailCommand request, CancellationToken cancellationToken)
    {
        PurchaseOrderDetail? existingPoDetail = await _dataContext.PurchaseOrderDetails
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if (existingPoDetail == null)
        {
            return null;
        }
        
        _dataContext.PurchaseOrderDetails.Remove(existingPoDetail);
        
        return existingPoDetail;
    }
}