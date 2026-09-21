namespace Squrl.App.Features.SalesOrderDetails.DTOs;

public class GetManySoDetailsDto
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public int TotalCount { get; }
    public IReadOnlyList<GetSoDetailDto>? SoDetails { get; }

    public GetManySoDetailsDto(int? pageNumber, int? pageSize, int totalCount, IReadOnlyList<GetSoDetailDto> soDetails)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        SoDetails = soDetails;
    }
}