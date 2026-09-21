namespace Squrl.App.Models;

public class SalesOrderDetail
{
    public required Guid Id { get; set; }
    public required SalesOrder SalesOrder { get; set; }
    public required int LineSequence { get; set; }
    public required Item Item { get; set; }
    public required decimal UnitPrice { get; set; }
    public required decimal Quantity { get; set; }
}