using NodaTime;
using Squrl.App.Features.SalesOrderDetails.DTOs;

namespace Squrl.App.Features.SalesOrders.DTOs;

public class UpdateSalesOrderDto
{
    public string? Customer { get; set; }
    public string? InvoiceNumber { get; set; }
    public List<UpdateSoDetailDto>? Details { get; set; }
    public Instant? DateOrdered { get; set; }
}