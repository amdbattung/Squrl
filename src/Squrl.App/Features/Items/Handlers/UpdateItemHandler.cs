using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.Items.Commands;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Handlers;

public class UpdateItemHandler : IRequestHandler<UpdateItemCommand, Item?>
{
    private readonly DataContext _dataContext;

    public UpdateItemHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<Item?> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        Item? existingItem = await _dataContext.Items.Include(i => i.Uom).FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        
        if (existingItem == null)
        {
            return null;
        }

        UpdateItemDto requestItem = request.Item;
        
        existingItem.Name = requestItem.Name ?? existingItem.Name;
        existingItem.Uom = requestItem.UomId == null
            ? existingItem.Uom
            : await _dataContext.UnitOfMeasures.FirstOrDefaultAsync(u => u.Id == requestItem.UomId, cancellationToken) ?? existingItem.Uom;
        existingItem.Description = requestItem.Description ?? existingItem.Description;
        existingItem.Quantity = requestItem.Quantity ?? existingItem.Quantity;
        existingItem.LowQuantityAlertThreshold = requestItem.LowQuantityAlertThreshold  ?? existingItem.LowQuantityAlertThreshold;
        existingItem.Locations = requestItem.Locations ?? existingItem.Locations;

        await _dataContext.SaveChangesAsync(cancellationToken);
        return existingItem;
    }
}