using DiscordTimeTracker.Application.Common;
using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Repositories;

namespace DiscordTimeTracker.Application.UseCases.GetEntriesOfToday;

public class GetEntriesOfTodayUseCase
{
    private readonly ITimeEntryRepository _repository;

    public GetEntriesOfTodayUseCase(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GetEntriesOfTodayResponse>> ExecuteAsync(GetEntriesOfTodayRequest request)
    {
        var utcNow = DateTime.UtcNow.Date;
        var startOfDay = utcNow;                   // 00:00:00 UTC hoje
        var endOfDay = utcNow.AddDays(1).AddTicks(-1); // 23:59:59.9999999 UTC hoje

        var entries = await _repository.GetEntriesByUserAndGuildAndDateRangeAsync(
            request.GuildId,
            request.UserId,
            startOfDay,
            endOfDay);

        if(entries?.Count == 0)
            return Result<GetEntriesOfTodayResponse>.Fail(":calendar: No entries found for today.");

        var message = ":calendar: No entries found for today.";

        if (entries?.Count > 0)
        {
            var formatted = string.Join("\n", entries.Select(e => $"• `{e.Timestamp:HH:mm}` - {e.Type}"));
            message =  $":calendar: Entries for today:\n{formatted}";
        }

        return Result<GetEntriesOfTodayResponse>.Ok(new GetEntriesOfTodayResponse(message));
    }
}