namespace Squrl.App.Features.Suppliers.DTOs;

public class GetManySuppliersDto
{
    public int? PageNumber { get; }
    public int? PageSize { get; }
    public int TotalCount { get; }
    public IReadOnlyList<GetSupplierDto>? Suppliers { get; }

    public GetManySuppliersDto(int? pageNumber, int? pageSize, int totalCount, IReadOnlyList<GetSupplierDto> suppliers)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        Suppliers = suppliers;
    }
}