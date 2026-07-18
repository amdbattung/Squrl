using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.PurchaseOrderDetails.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Handlers;

public class GetPoDetailByIdHandler : IRequestHandler<GetPoDetailByIdQuery, PurchaseOrderDetail?>
{
    private readonly DataContext _dataContext;

    public GetPoDetailByIdHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<PurchaseOrderDetail?> Handle(GetPoDetailByIdQuery request, CancellationToken cancellationToken)
    {
        PurchaseOrderDetail? existingPoDetail = await _dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .Include(p => p.Item)
            .Where(i => i.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return existingPoDetail;
    }
}