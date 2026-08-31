namespace Squrl.App.Features.PurchaseOrderDetails.DTOs;

public class UpdatePoDetailDto
{
    public Guid? PurchaseOrderId { get; set; }
    public int? LineSequence { get; set; }
    public Guid? ItemId { get; set; }
    public decimal? Quantity { get; set; }
}