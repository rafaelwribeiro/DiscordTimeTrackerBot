using DiscordTimeTracker.Application.Common;
using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Enums;
using DiscordTimeTracker.Domain.Repositories;

namespace DiscordTimeTracker.Application.UseCases.ManualEntry;

public class ManualEntryUseCase
{
    private readonly ITimeEntryRepository _repository;

    public ManualEntryUseCase(ITimeEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ManualEntryResponse>> ExecuteAsync(ManualEntryRequest request)
    {
        var lastEntry = await _repository.GetLastEntryByUserAsync(request.GuildId, request.UserId);
        if (lastEntry?.Type == request.Type)
            return Result<ManualEntryResponse>.Fail($":head_shaking_horizontally: You are already {(request.Type == TimeEntryType.ClockIn ? "In" : "Out")}");

        var entry = new TimeEntry
        {
            GuildId = request.GuildId,
            UserId = request.UserId,
            UserName = request.UserName,
            Timestamp = request.Timestamp.ToUniversalTime(),
            Type = request.Type
        };

        await _repository.AddAsync(entry);

        var message = $"📌 Manual entry ({request.Type}) added for {request.UserName} at {entry.Timestamp:yyyy-MM-dd HH:mm:ss} UTC";

        return Result<ManualEntryResponse>.Ok(new ManualEntryResponse(message));
    }
}
