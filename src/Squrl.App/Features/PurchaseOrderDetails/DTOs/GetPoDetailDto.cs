namespace Squrl.App.Features.PurchaseOrderDetails.DTOs;

public class GetPoDetailDto
{
    public Guid? Id { get; }
    public Guid? PurchaseOrderId { get; }
    public int? LineSequence { get; }
    public Guid? ItemId { get; }
    public decimal? Quantity { get; }
    public decimal? UnitPrice { get; set; }

    public GetPoDetailDto(Guid? id,
        Guid? purchaseOrderId,
        int? lineSequence,
        Guid? itemId,
        decimal? quantity,
        decimal? unitPrice)
    {
        Id = id;
        PurchaseOrderId = purchaseOrderId;
        LineSequence = lineSequence;
        ItemId = itemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}