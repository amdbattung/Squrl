using NodaTime;
using Squrl.App.Enums;

namespace Squrl.App.Features.PurchaseOrders.DTOs;

public class GetPurchaseOrderDto
{
    public Guid? Id { get; }
    public Guid? SupplierId { get; }
    public PurchaseOrderStatus? Status { get; }
    public Instant? DateOrdered { get; }
    public Instant? DateRequired { get; }
    public Instant? DateShipped { get; }

    public GetPurchaseOrderDto(Guid? id,
        Guid? supplierId,
        PurchaseOrderStatus? status,
        Instant? dateOrdered,
        Instant? dateRequired,
        Instant? dateShipped)
    {
        Id = id;
        SupplierId = supplierId;
        Status = status;
        DateOrdered = dateOrdered;
        DateRequired = dateRequired;
        DateShipped = dateShipped;
    }
}