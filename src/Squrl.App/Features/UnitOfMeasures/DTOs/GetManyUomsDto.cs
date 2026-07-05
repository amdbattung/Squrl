namespace Squrl.App.Features.UnitOfMeasures.DTOs;

public class GetManyUomsDto
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public int? TotalCount { get; }
    public IReadOnlyList<GetUomDto>? Uoms { get; }

    public GetManyUomsDto(int pageNumber, int pageSize, int totalCount, IReadOnlyList<GetUomDto> uoms)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        Uoms = uoms;
    }
}