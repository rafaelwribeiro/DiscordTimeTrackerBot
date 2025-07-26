using DiscordTimeTracker.Application.Common;
using DiscordTimeTracker.Application.UseCases.ClockIn;
using DiscordTimeTracker.Application.Validators;
using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Enums;
using DiscordTimeTracker.Domain.Repositories;

namespace DiscordTimeTracker.Application.UseCases.ClockOut;

public class ClockOutUseCase
{
    private readonly ITimeEntryRepository _repository;

    public ClockOutUseCase(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ClockOutResponse>> ExecuteAsync(ClockOutRequest request)
    {
        var lastEntry = await _repository.GetLastEntryByUserAsync(request.GuildId, request.UserId);
        if (!TimeEntryValidator.CanClockOut(lastEntry))
            return Result<ClockOutResponse>.Fail($":head_shaking_horizontally: You are already Out");

        var entry = new TimeEntry
        {
            GuildId = request.GuildId,
            UserId = request.UserId,
            UserName = request.UserName,
            Timestamp = DateTime.UtcNow,
            Type = TimeEntryType.ClockOut
        };

        await _repository.AddAsync(entry);

        var message = $"🕒 Clock-out registered for {request.UserName} at {entry.Timestamp:HH:mm:ss} UTC";

        return Result<ClockOutResponse>.Ok(new ClockOutResponse(message));
    }
}
