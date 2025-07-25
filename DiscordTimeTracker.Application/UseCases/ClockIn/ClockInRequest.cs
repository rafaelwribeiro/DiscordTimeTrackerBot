namespace DiscordTimeTracker.Application.UseCases.ClockIn;

public class ClockInRequest
{
    public string GuildId { get; }
    public string UserId { get; }
    public string UserName { get; }

    public ClockInRequest(string guildId, string userId, string userName)
    {
        GuildId = guildId;
        UserId = userId;
        UserName = userName;
    }
}
