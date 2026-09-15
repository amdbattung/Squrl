using System.Linq.Expressions;
using Squrl.App.Features.SalesOrderDetails.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.SalesOrderDetails.Mapping;

public class SoDetailMapper
{
    public static GetSoDetailDto ToDto(SalesOrderDetail soDetail)
    {
        return new GetSoDetailDto(
            soDetail.Id,
            soDetail.SalesOrder.Id,
            soDetail.LineSequence,
            soDetail.Item.Id,
            soDetail.UnitPrice,
            soDetail.Quantity
        );
    }
    
    public static Expression<Func<SalesOrderDetail, GetSoDetailDto>> ToDtoExpression
    {
        get
        {
            return soDetail => new GetSoDetailDto(
                soDetail.Id,
                soDetail.SalesOrder.Id,
                soDetail.LineSequence,
                soDetail.Item.Id,
                soDetail.UnitPrice,
                soDetail.Quantity
            );
        }
    }
}