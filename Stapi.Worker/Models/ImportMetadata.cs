namespace Stapi.Worker.Models;

public sealed record ImportMetadata
{
    public long Id { get; set; }

    public string ExternalId { get; set; } = default!;

    public string Type { get; set; } = default!;

    public string Data { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}