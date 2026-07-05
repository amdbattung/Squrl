namespace Squrl.App.Features.Items.DTOs;

public class UpdateItemDto
{
    public string? Name { get; set; }
    public Guid? UomId { get; set; }
    public string? Description { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? LowQuantityAlertThreshold { get; set; }
    public List<string>? Locations { get; set; }
}