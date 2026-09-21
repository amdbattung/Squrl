using NodaTime;
using Squrl.App.Features.SalesOrderDetails.DTOs;

namespace Squrl.App.Features.SalesOrders.DTOs;

public class CreateSalesOrderDto
{
    public string? Customer { get; set; }
    public string? InvoiceNumber { get; set; }
    public List<CreateSoDetailDto>? Details { get; set; }
    public Instant? DateOrdered { get; set; }
}