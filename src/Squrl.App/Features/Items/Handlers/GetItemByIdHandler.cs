using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.Items.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Handlers;

public class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, Item?>
{
    private readonly DataContext _dataContext;

    public GetItemByIdHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<Item?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        Item? existingItem = await _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .Where(i => i.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return existingItem;
    }
}