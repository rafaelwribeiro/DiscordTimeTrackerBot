namespace DiscordTimeTracker.Application.UseCases.GenerateMonthlyReport;

public class GenerateMonthlyReportRequest
{
    public string GuildId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public int Month { get; set; }
    public int Year { get; set; }

    public GenerateMonthlyReportRequest(
        string guildId,
        string userId,
        string userName,
        int month,
        int year)
    {
        GuildId = guildId;
        UserId = userId;
        UserName = userName;
        Month = month;
        Year = year;
    }
}

