namespace Squrl.App.Features.SalesOrders.DTOs;

public class GetManySalesOrdersDto
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public int TotalCount { get; }
    public IReadOnlyList<GetSalesOrderDto>? SalesOrders { get; }

    public GetManySalesOrdersDto(int? pageNumber, int? pageSize, int totalCount, IReadOnlyList<GetSalesOrderDto> salesOrders)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        SalesOrders = salesOrders;
    }
}