using NodaTime;

namespace Squrl.App.Common;

public class Alert
{
    public required Guid ItemId { get; set; }
    public string? Message { get; set; }
    public Instant? Timestamp { get; set; }
}