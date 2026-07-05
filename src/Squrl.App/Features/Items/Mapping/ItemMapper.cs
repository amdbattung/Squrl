using System.Linq.Expressions;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.Items.Mapping;

public class ItemMapper
{
    public static GetItemDto ToDto(Item item)
    {
        return new GetItemDto(
            item.Id,
            item.Name,
            item.Uom.Id,
            item.Description,
            item.Quantity,
            item.LowQuantityAlertThreshold,
            item.Locations.ToList()
        );
    }
    
    public static Expression<Func<Item, GetItemDto>> ToDtoExpression
    {
        get
        {
            return item => new GetItemDto(
                item.Id,
                item.Name,
                item.Uom.Id,
                item.Description,
                item.Quantity,
                item.LowQuantityAlertThreshold,
                item.Locations.ToList()
            );
        }
    }
}