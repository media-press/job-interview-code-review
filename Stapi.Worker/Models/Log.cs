namespace Stapi.Worker.Models;

public class Log
{
    public long Id { get; set; }

    public string AppName { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Type { get; set; } = default!;

    public string Message { get; set; } = default!;
}