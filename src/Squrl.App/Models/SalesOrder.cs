using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Squrl.App.Models;

public class SalesOrder
{
    public required Guid Id { get; set; }
    [MaxLength(255)]
    public string? Customer { get; set; }
    [MaxLength(255)]
    public string? InvoiceNumber { get; set; }
    public Instant DateOrdered { get; set; }
}