using System.ComponentModel.DataAnnotations;

namespace Squrl.App.Models;

public class Item                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
{
    public required Guid Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }
    public required UnitOfMeasure Uom { get; set; }
    [MaxLength(255)]
    public string? Description { get; set; }
    [MaxLength(255)]
    public string? Image { get; set; }
    public required decimal Quantity { get; set; }
    public decimal? LowQuantityAlertThreshold  { get; set; }
    public required ICollection<string> Locations { get; set; }
}