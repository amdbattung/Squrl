using NodaTime;
using Squrl.App.Enums;

namespace Squrl.App.Models;

public class PurchaseOrder
{
    public required Guid Id { get; set; }
    public Supplier? Supplier { get; set; }
    public required PurchaseOrderStatus Status { get; set; }
    public Instant DateOrdered { get; set; }
    public Instant? DateRequired { get; set; }
    public Instant? DateShipped { get; set; }
}