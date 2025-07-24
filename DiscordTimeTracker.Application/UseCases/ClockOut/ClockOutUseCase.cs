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

    public async Task<ClockOutResponse> ExecuteAsync(ClockOutRequest request)
    {
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

        return new ClockOutResponse(message);
    }
}
