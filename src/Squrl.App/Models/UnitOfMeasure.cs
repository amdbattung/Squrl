using System.ComponentModel.DataAnnotations;

namespace Squrl.App.Models;

public class UnitOfMeasure
{
    public required Guid Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }
    [MaxLength(255)]
    public required string Code { get; set; }
    [MaxLength(255)]
    public string? Description { get; set; }
}