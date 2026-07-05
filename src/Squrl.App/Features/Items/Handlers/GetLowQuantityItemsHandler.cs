using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Data;
using Squrl.App.Features.Items.Queries;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Handlers;

public class GetLowQuantityItemsHandler : IRequestHandler<GetLowQuantityItemsQuery, IReadOnlyList<Item>>
{
    private readonly DataContext _dataContext;

    public GetLowQuantityItemsHandler(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IReadOnlyList<Item>> Handle(GetLowQuantityItemsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Item> existingItems = await _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .Where(i => i.Quantity <= i.LowQuantityAlertThreshold)
            .OrderBy(i => EF.Property<Instant>(i, "DateCreated"))
            .ThenBy(i => i.Name)
            .ToListAsync(cancellationToken);

        return existingItems;
    }
}