using NodaTime;

namespace Squrl.App.Features.SalesOrders.DTOs;

public class GetSalesOrderDto
{
    public Guid? Id { get; }
    public string? Customer { get; }
    public string? InvoiceNumber { get; }
    public Instant? DateOrdered { get; }

    public GetSalesOrderDto(Guid? id,
        string? customer,
        string? invoiceNumber,
        Instant? dateOrdered)
    {
        Id = id;
        Customer = customer;
        InvoiceNumber = invoiceNumber;
        DateOrdered = dateOrdered;
    }
}