namespace DiscordTimeTracker.Application.UseCases.ClockIn;

public class ClockInRequest
{
    public ulong GuildId { get; }
    public ulong UserId { get; }
    public string UserName { get; }

    public ClockInRequest(ulong guildId, ulong userId, string userName)
    {
        GuildId = guildId;
        UserId = userId;
        UserName = userName;
    }
}
