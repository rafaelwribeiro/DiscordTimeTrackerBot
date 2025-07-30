namespace DiscordTimeTracker.Application.UseCases.GenerateMonthlyReport;

public class GenerateMonthlyReportRequest
{
    public string GuildId { get; init; }
    public string UserId { get; init; }
    public string UserName { get; set; }
    public int Year { get; init; }
    public int Month { get; init; }

    public GenerateMonthlyReportRequest(string guildId, string userId, string userName, int year, int month)
    {
        GuildId = guildId;
        UserId = userId;
        UserName = userName;
        Year = year;
        Month = month;
    }
}


