namespace Squrl.App.Features.SalesOrderDetails.DTOs;

public class CreateSoDetailDto
{
    public Guid? SalesOrderId { get; set; }
    public int? LineSequence { get; set; }
    public Guid? ItemId { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Quantity { get; set; }
}