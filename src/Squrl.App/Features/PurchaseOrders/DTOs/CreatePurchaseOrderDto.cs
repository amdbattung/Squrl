using NodaTime;
using Squrl.App.Enums;

namespace Squrl.App.Features.PurchaseOrders.DTOs;

public class CreatePurchaseOrderDto
{
    public Guid? SupplierId { get; set; }
    public PurchaseOrderStatus? Status { get; set; }
    public Instant? DateOrdered { get; set; }
    public Instant? DateRequired { get; set; }
    public Instant? DateShipped { get; set; }
}