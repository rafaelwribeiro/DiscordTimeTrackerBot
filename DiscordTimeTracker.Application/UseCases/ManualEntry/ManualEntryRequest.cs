using DiscordTimeTracker.Domain.Enums;

namespace DiscordTimeTracker.Application.UseCases.ManualEntry;

public class ManualEntryRequest
{
    public string GuildId { get; }
    public string UserId { get; }
    public string UserName { get; }
    public DateTime Timestamp { get; }
    public TimeEntryType Type { get; }

    public ManualEntryRequest(string guildId, string userId, string userName, DateTime timestamp, TimeEntryType type)
    {
        GuildId = guildId;
        UserId = userId;
        UserName = userName;
        Timestamp = timestamp;
        Type = type;
    }
}
