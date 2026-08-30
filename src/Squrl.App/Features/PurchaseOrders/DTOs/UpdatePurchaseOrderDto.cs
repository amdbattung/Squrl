using NodaTime;
using Squrl.App.Enums;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;

namespace Squrl.App.Features.PurchaseOrders.DTOs;

public class UpdatePurchaseOrderDto
{
    public Guid? SupplierId { get; set; }
    public PurchaseOrderStatus? Status { get; set; }
    public List<UpdatePoDetailDto>? PurchaseOrderDetails { get; set; }
    public Instant? DateOrdered { get; set; }
    public Instant? DateRequired { get; set; }
    public Instant? DateShipped { get; set; }
}