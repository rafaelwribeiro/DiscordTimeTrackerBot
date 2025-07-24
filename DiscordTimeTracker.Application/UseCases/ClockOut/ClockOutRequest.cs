namespace DiscordTimeTracker.Application.UseCases.ClockOut;

public class ClockOutRequest
{
    public ulong GuildId { get; }
    public ulong UserId { get; }
    public string UserName { get; }

    public ClockOutRequest(ulong guildId, ulong userId, string userName)
    {
        GuildId = guildId;
        UserId = userId;
        UserName = userName;
    }
}
