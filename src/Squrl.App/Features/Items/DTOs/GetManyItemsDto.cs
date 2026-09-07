namespace Squrl.App.Features.Items.DTOs;

public class GetManyItemsDto
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public int TotalCount { get; }
    public IReadOnlyList<GetItemDto>? Items { get; }

    public GetManyItemsDto(int? pageNumber, int? pageSize, int totalCount, IReadOnlyList<GetItemDto> items)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items;
    }
}