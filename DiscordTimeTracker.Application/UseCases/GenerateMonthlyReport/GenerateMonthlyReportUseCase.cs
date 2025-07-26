using DiscordTimeTracker.Application.Common;
using DiscordTimeTracker.Application.DTOs;
using DiscordTimeTracker.Domain.Repositories;
using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Enums;

namespace DiscordTimeTracker.Application.UseCases.GenerateMonthlyReport;

public class GenerateMonthlyReportUseCase
{
    private readonly ITimeEntryRepository _repository;

    public GenerateMonthlyReportUseCase(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GenerateMonthlyReportResponse>> ExecuteAsync(GenerateMonthlyReportRequest request)
    {
        if (request.Month < 1 || request.Month > 12)
            return Result<GenerateMonthlyReportResponse>.Fail("Invalid month.");

        if (request.Year < 2000 || request.Year > DateTime.UtcNow.Year)
            return Result<GenerateMonthlyReportResponse>.Fail("Invalid year.");

        var entries = await _repository.GetEntriesByUserAndGuildAndMonthAsync(
            request.GuildId,
            request.UserId,
            request.Year,
            request.Month
        );

        if (entries == null || entries.Count == 0)
            return Result<GenerateMonthlyReportResponse>.Ok(new GenerateMonthlyReportResponse(request.UserName, request.Month, request.Year, []));

        var grouped = entries
            .GroupBy(e => DateOnly.FromDateTime(e.Timestamp.ToLocalTime()))
            .Select(group =>
            {
                var ordered = group.OrderBy(e => e.Timestamp).ToList();
                var worked = CalculateWorkedTime(ordered);
                var entryDtos = ordered.Select(e => new TimeEntryDto(e.Timestamp.ToLocalTime(), e.Type.ToString())).ToList();

                return new GenerateMonthlyReportResponse.DailySummary(
                    group.Key,
                    worked,
                    entryDtos
                );
            })
            .OrderBy(s => s.Date)
            .ToList();

        var response = new GenerateMonthlyReportResponse(
            request.UserName,
            request.Month,
            request.Year,
            grouped
        );

        return Result<GenerateMonthlyReportResponse>.Ok(response);
    }

    private static TimeSpan CalculateWorkedTime(List<TimeEntry> entries)
    {
        TimeSpan total = TimeSpan.Zero;
        for (int i = 0; i < entries.Count - 1; i += 2)
        {
            var start = entries[i];
            var end = entries[i + 1];

            if (start.Type == TimeEntryType.ClockIn && end.Type == TimeEntryType.ClockOut)
            {
                total += end.Timestamp - start.Timestamp;
            }
        }
        return total;
    }
}
