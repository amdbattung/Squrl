using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.SalesOrderDetails.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Handlers;

public class DeleteSoDetailHandler : IRequestHandler<DeleteSoDetailCommand, SalesOrderDetail?>
{
    private readonly DataContext _dataContext;

    public DeleteSoDetailHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<SalesOrderDetail?> Handle(DeleteSoDetailCommand request, CancellationToken cancellationToken)
    {
        SalesOrderDetail? existingSoDetail = await _dataContext.SalesOrderDetails
            .Include(s => s.SalesOrder)
            .Include(s => s.Item)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        
        if (existingSoDetail == null)
        {
            return null;
        }
        
        _dataContext.SalesOrderDetails.Remove(existingSoDetail);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? existingSoDetail : null;
    }
}