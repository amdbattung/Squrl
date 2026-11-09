namespace Squrl.App.Features.SalesOrderDetails.DTOs;

public class GetSoDetailDto
{
    public Guid? Id { get; }
    public Guid? SalesOrderId { get; }
    public int? LineSequence { get; }
    public Guid? ItemId { get; }
    public decimal? Quantity { get; }
    public decimal? UnitPrice { get; set; }

    public GetSoDetailDto(Guid? id,
        Guid? salesOrderId,
        int? lineSequence,
        Guid? itemId,
        decimal? quantity,
        decimal? unitPrice)
    {
        Id = id;
        SalesOrderId = salesOrderId;
        LineSequence = lineSequence;
        ItemId = itemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}