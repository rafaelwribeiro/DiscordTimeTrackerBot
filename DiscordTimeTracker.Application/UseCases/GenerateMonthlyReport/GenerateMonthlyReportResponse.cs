using DiscordTimeTracker.Application.DTOs;

namespace DiscordTimeTracker.Application.UseCases.GenerateMonthlyReport;

public class GenerateMonthlyReportResponse
{
    public string UserName { get; }
    public int Month { get; }
    public int Year { get; }
    public IReadOnlyList<DailySummary> DailySummaries { get; }

    public GenerateMonthlyReportResponse(
        string userName,
        int month,
        int year,
        IReadOnlyList<DailySummary> dailySummaries)
    {
        UserName = userName;
        Month = month;
        Year = year;
        DailySummaries = dailySummaries;
    }

    public class DailySummary
    {
        public DateOnly Date { get; }
        public TimeSpan TotalWorked { get; }
        public IReadOnlyList<TimeEntryDto> Entries { get; }

        public DailySummary(DateOnly date, TimeSpan totalWorked, IReadOnlyList<TimeEntryDto> entries)
        {
            Date = date;
            TotalWorked = totalWorked;
            Entries = entries;
        }
    }
}
