namespace Squrl.App.Models;

public class PurchaseOrderDetail
{
    public required Guid Id { get; set; }
    public required PurchaseOrder PurchaseOrder { get; set; }
    public required Item PurchaseOrderItem { get; set; }
    public required decimal Quantity { get; set; }
}