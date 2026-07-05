using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.Items.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Handlers;

public class DeleteItemHandler : IRequestHandler<DeleteItemCommand, Item?>
{
    private readonly DataContext _dataContext;

    public DeleteItemHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<Item?> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        Item? existingItem = await _dataContext.Items
            .Include(i => i.Uom)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        
        if (existingItem == null)
        {
            return null;
        }
        
        _dataContext.Items.Remove(existingItem);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? existingItem : null;
    }
}