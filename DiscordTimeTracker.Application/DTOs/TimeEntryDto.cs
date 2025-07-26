namespace DiscordTimeTracker.Application.DTOs;

public class TimeEntryDto
{
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;

    public TimeEntryDto(DateTime timestamp, string type)
    {
        Timestamp = timestamp;
        Type = type;
    }
}
