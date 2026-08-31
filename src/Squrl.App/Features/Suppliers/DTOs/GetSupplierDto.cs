namespace Squrl.App.Features.Suppliers.DTOs;

public class GetSupplierDto
{
    public Guid? Id { get; }
    public string? Name { get; }
    public string? Description { get; }

    public GetSupplierDto(Guid? id,
        string? name,
        string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}