using System.Linq.Expressions;
using Squrl.App.Features.PurchaseOrders.DTOs;
using Squrl.App.Models;

namespace Squrl.App.Features.PurchaseOrders.Mapping;

public class PurchaseOrderMapper
{
    public static GetPurchaseOrderDto ToDto(PurchaseOrder purchaseOrder)
    {
        return new GetPurchaseOrderDto(
            purchaseOrder.Id,
            purchaseOrder.Supplier?.Id,
            purchaseOrder.Status,
            purchaseOrder.DateOrdered,
            purchaseOrder.DateRequired,
            purchaseOrder.DateShipped
        );
    }
    
    public static Expression<Func<PurchaseOrder, GetPurchaseOrderDto>> ToDtoExpression
    {
        get
        {
            return purchaseOrder => new GetPurchaseOrderDto(
                purchaseOrder.Id,
                purchaseOrder.Supplier != null ? purchaseOrder.Supplier.Id : null,
                purchaseOrder.Status,
                purchaseOrder.DateOrdered,
                purchaseOrder.DateRequired,
                purchaseOrder.DateShipped
            );
        }
    }
}