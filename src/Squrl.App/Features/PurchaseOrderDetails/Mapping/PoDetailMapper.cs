using System.Linq.Expressions;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrderDetails.Mapping;

public class PoDetailMapper
{
    public static GetPoDetailDto ToDto(PurchaseOrderDetail poDetail)
    {
        return new GetPoDetailDto(
            poDetail.Id,
            poDetail.PurchaseOrder.Id,
            poDetail.LineSequence,
            poDetail.Item.Id,
            poDetail.Quantity
        );
    }
    
    public static Expression<Func<PurchaseOrderDetail, GetPoDetailDto>> ToDtoExpression
    {
        get
        {
            return poDetail => new GetPoDetailDto(
                poDetail.Id,
                poDetail.PurchaseOrder.Id,
                poDetail.LineSequence,
                poDetail.Item.Id,
                poDetail.Quantity
            );
        }
    }
}