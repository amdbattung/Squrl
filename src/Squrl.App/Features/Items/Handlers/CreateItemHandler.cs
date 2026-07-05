using MediatR;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Features.Items.Commands;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Handlers;

public class CreateItemHandler : IRequestHandler<CreateItemCommand, Item?>
{
    private readonly DataContext _dataContext;

    public CreateItemHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Item?> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        UnitOfMeasure? uom = await _dataContext.UnitOfMeasures.FirstOrDefaultAsync(u => u.Id == request.Item.UomId, cancellationToken);
        
        if (uom == null)
        {
            return null;
        }
        
        Item newItem = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Item.Name ?? "",
            Uom = uom,
            Description = request.Item.Description,
            Quantity = request.Item.Quantity ?? 0m,
            LowQuantityAlertThreshold =  request.Item.LowQuantityAlertThreshold,
            Locations = request.Item.Locations ?? new List<string>()
        };
        
        await _dataContext.Items.AddAsync(newItem, cancellationToken);
        
        return await _dataContext.SaveChangesAsync(cancellationToken) > 0 ? newItem : null;
    }
}