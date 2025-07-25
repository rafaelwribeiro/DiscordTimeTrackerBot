using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Repositories;

namespace DiscordTimeTracker.Application.UseCases.GetEntriesOfToday;

public class GetEntriesOfTodayRequest
{
    public string GuildId { get; }
    public string UserId { get; }

    public GetEntriesOfTodayRequest(string guildId, string userId)
    {
        GuildId = guildId;
        UserId = userId;
    }
}
