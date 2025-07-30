using DiscordTimeTracker.Domain.Enums;

namespace DiscordTimeTracker.Application.DTOs;

public class TimeEntryDto
{
    public TimeEntryDto(DateTime timestamp, TimeEntryType type)
    {
        Timestamp = timestamp;
        Type = type;
    }

    public DateTime Timestamp { get; set; }
    public TimeEntryType Type { get; set; }
}
