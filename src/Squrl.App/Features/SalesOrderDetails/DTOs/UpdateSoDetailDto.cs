namespace Squrl.App.Features.SalesOrderDetails.DTOs;

public class UpdateSoDetailDto
{
    public Guid? SalesOrderId { get; set; }
    public int? LineSequence { get; set; }
    public Guid? ItemId { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Quantity { get; set; }
}