using NodaTime;

namespace Squrl.App.Features.SalesOrders.DTOs;

public class CreateSalesOrderDto
{
    public string? Customer { get; set; }
    public string? InvoiceNumber { get; set; }
    public Instant? DateOrdered { get; set; }
}