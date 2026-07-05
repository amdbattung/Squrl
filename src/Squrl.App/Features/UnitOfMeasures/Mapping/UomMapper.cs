using System.Linq.Expressions;
using Squrl.App.Features.Items.DTOs;
using Squrl.App.Features.UnitOfMeasures.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.UnitOfMeasures.Mapping;

public class UomMapper
{
    public static GetUomDto ToDto(UnitOfMeasure uom)
    {
        return new GetUomDto(
            uom.Id,
            uom.Name,
            uom.Code,
            uom.Description
        );
    }
    
    public static Expression<Func<UnitOfMeasure, GetUomDto>> ToDtoExpression
    {
        get
        {
            return uom => new GetUomDto(
                uom.Id,
                uom.Name,
                uom.Code,
                uom.Description
            );
        }
    }
}