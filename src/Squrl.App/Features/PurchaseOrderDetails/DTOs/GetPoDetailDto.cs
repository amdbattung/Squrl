namespace Squrl.App.Features.PurchaseOrderDetails.DTOs;

public class GetPoDetailDto
{
    public Guid? Id { get; }
    public Guid? PurchaseOrderId { get; }
    public Guid? ItemId { get; }
    public decimal? Quantity { get; }

    public GetPoDetailDto(Guid? id,
        Guid? purchaseOrderId,
        Guid? itemId,
        decimal? quantity)
    {
        Id = id;
        PurchaseOrderId = purchaseOrderId;
        ItemId = itemId;
        Quantity = quantity;
    }
}