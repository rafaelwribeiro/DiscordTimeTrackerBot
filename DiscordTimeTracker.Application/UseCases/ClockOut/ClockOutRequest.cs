namespace DiscordTimeTracker.Application.UseCases.ClockOut;

public class ClockOutRequest
{
    public string GuildId { get; }
    public string UserId { get; }
    public string UserName { get; }

    public ClockOutRequest(string guildId, string userId, string userName)
    {
        GuildId = guildId;
        UserId = userId;
        UserName = userName;
    }
}
