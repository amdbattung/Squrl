namespace Squrl.App.Features.PurchaseOrders.DTOs;

public class GetManyPurchaseOrdersDto
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public int TotalCount { get; }
    public IReadOnlyList<GetPurchaseOrderDto>? PurchaseOrders { get; }

    public GetManyPurchaseOrdersDto(int? pageNumber, int? pageSize, int totalCount, IReadOnlyList<GetPurchaseOrderDto> purchaseOrders)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        PurchaseOrders = purchaseOrders;
    }
}