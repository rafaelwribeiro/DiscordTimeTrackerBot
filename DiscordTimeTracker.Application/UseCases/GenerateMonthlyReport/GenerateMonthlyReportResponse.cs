namespace DiscordTimeTracker.Application.UseCases.GenerateMonthlyReport;

public class GenerateMonthlyReportResponse
{
    public string FileName { get; init; }
    public byte[] FileBytes { get; init; }
    public int EntryCount { get; set; }
}

