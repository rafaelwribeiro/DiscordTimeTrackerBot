using DiscordTimeTracker.Application.Common;
using DiscordTimeTracker.Application.DTOs;
using DiscordTimeTracker.Application.Interfaces;
using DiscordTimeTracker.Domain.Repositories;

namespace DiscordTimeTracker.Application.UseCases.GenerateMonthlyReport;

public class GenerateMonthlyReportUseCase
{
    private readonly ITimeEntryRepository _repository;
    private readonly IMonthlyReportPdfGenerator _pdfGenerator;

    public GenerateMonthlyReportUseCase(ITimeEntryRepository repository, IMonthlyReportPdfGenerator pdfGenerator)
    {
        _repository = repository;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<Result<GenerateMonthlyReportResponse>> ExecuteAsync(GenerateMonthlyReportRequest request)
    {
        var start = new DateTime(request.Year, request.Month, 1);
        var end = start.AddMonths(1).AddTicks(-1);

        var entries = await _repository.GetEntriesByUserAndGuildAndDateRangeAsync(
            request.GuildId,
            request.UserId,
            start,
            end
        );

        if (entries.Count == 0)
            return Result<GenerateMonthlyReportResponse>.Fail("No entries found for this month.");

        var entryDtos = entries
            .OrderBy(e => e.Timestamp)
            .Select(e => new TimeEntryDto(e.Timestamp, e.Type))
            .ToList();

        TimeSpan totalWorked = TimeSpan.Zero;
        for (int i = 0; i < entryDtos.Count - 1; i++)
        {
            if (entryDtos[i].Type == Domain.Enums.TimeEntryType.ClockIn &&
                entryDtos[i + 1].Type == Domain.Enums.TimeEntryType.ClockOut)
            {
                totalWorked += entryDtos[i + 1].Timestamp - entryDtos[i].Timestamp;
                i++; // skip the next entry
            }
        }

        var pdfBytes = _pdfGenerator.Generate(request.UserName, request.Year, request.Month, entryDtos, totalWorked);

        return Result<GenerateMonthlyReportResponse>.Ok(new GenerateMonthlyReportResponse
        {
            FileName = $"report_{request.Year}_{request.Month:D2}.pdf",
            FileBytes = pdfBytes,
            EntryCount = entryDtos.Count
        });
    }
}
