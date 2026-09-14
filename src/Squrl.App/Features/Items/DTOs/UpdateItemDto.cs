namespace Squrl.App.Features.Items.DTOs;

public class UpdateItemDto
{
    public string? Name { get; set; }
    public Guid? UomId { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public decimal? ListPrice { get; set; }
    public decimal? RetailPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? LowQuantityAlertThreshold { get; set; }
    public List<string>? Locations { get; set; }
}