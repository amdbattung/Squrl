namespace Squrl.App.Features.PurchaseOrderDetails.DTOs;

public class GetManyPoDetailsDto
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public int? TotalCount { get; }
    public IReadOnlyList<GetPoDetailDto>? PoDetails { get; }

    public GetManyPoDetailsDto(int pageNumber, int pageSize, int totalCount, IReadOnlyList<GetPoDetailDto> poDetails)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        PoDetails = poDetails;
    }
}