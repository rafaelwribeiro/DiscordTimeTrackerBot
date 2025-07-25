using DiscordTimeTracker.Domain.Enums;

namespace DiscordTimeTracker.Domain.Entities;

public class TimeEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string GuildId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public TimeEntryType Type { get; set; }
}
