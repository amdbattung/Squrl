namespace Squrl.App.Features.UnitOfMeasures.DTOs;

public class GetUomDto
{
    public Guid? Id { get; }
    public string? Name { get; }
    public string? Code { get; }
    public string? Description { get; }

    public GetUomDto(Guid? id,
        string? name,
        string? code,
        string? description)
    {
        Id = id;
        Name = name;
        Code = code;
        Description = description;
    }
}