using NodaTime;

namespace Squrl.App.Features.SalesOrders.DTOs;

public class GetSalesOrderDto
{
    public string? Customer { get; }
    public string? InvoiceNumber { get; }
    public Instant? DateOrdered { get; }

    public GetSalesOrderDto(string? customer,
        string? invoiceNumber,
        Instant? dateOrdered)
    {
        Customer = customer;
        InvoiceNumber = invoiceNumber;
        DateOrdered = dateOrdered;
    }
}