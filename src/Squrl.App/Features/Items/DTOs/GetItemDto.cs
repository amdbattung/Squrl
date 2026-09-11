namespace Squrl.App.Features.Items.DTOs;

public class GetItemDto
{
    public Guid? Id { get; }
    public string? Name { get; }
    public Guid? UomId { get; }
    public string? Description { get; }
    public string? Image { get; }
    public decimal? Quantity { get; }
    public decimal? LowQuantityAlertThreshold  { get; }
    public IReadOnlyList<string>? Locations { get; }
    
    public GetItemDto(Guid? id,
        string? name,
        Guid? uomId,
        string? description,
        string? image,
        decimal? quantity,
        decimal? lowQuantityAlertThreshold,
        IReadOnlyList<string>? locations)
    {
        Id = id;
        Name = name;
        UomId = uomId;
        Description = description;
        Image = image;
        Quantity = quantity;
        LowQuantityAlertThreshold = lowQuantityAlertThreshold;
        Locations = locations;
    }
}