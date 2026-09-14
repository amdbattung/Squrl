using NodaTime;

namespace Squrl.App.Features.SalesOrders.DTOs;

public class UpdateSalesOrderDto
{
    public string? Customer { get; set; }
    public string? InvoiceNumber { get; set; }
    public Instant? DateOrdered { get; set; }
}