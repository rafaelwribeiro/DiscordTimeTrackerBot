using DiscordTimeTracker.Application.DTOs;

namespace DiscordTimeTracker.Application.Interfaces;

public interface IMonthlyReportPdfGenerator
{
    byte[] Generate(string userName, int year, int month, List<TimeEntryDto> entries, TimeSpan totalWorked);
}
