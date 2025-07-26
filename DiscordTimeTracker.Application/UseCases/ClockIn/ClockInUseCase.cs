using DiscordTimeTracker.Application.Common;
using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Enums;
using DiscordTimeTracker.Domain.Repositories;

namespace DiscordTimeTracker.Application.UseCases.ClockIn;

public class ClockInUseCase
{
    private readonly ITimeEntryRepository _repository;

    public ClockInUseCase(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ClockInResponse>> ExecuteAsync(ClockInRequest request)
    {
        var lastEntry = await _repository.GetLastEntryByUserAsync(request.GuildId, request.UserId);
        if (lastEntry?.Type == TimeEntryType.ClockIn)
            return Result<ClockInResponse>.Fail(":head_shaking_horizontally: You are already clocked in.");

        var entry = new TimeEntry
        {
            GuildId = request.GuildId,
            UserId = request.UserId,
            UserName = request.UserName,
            Timestamp = DateTime.UtcNow,
            Type = TimeEntryType.ClockIn
        };

        await _repository.AddAsync(entry);

        var message = $"✅ Clock-in registered for {request.UserName} at {entry.Timestamp:HH:mm:ss} UTC";

        return Result<ClockInResponse>.Ok(new ClockInResponse(message));
    }
}
