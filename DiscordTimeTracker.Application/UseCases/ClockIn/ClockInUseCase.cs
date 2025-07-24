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

    public async Task<ClockInResponse> ExecuteAsync(ClockInRequest request)
    {
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

        return new ClockInResponse(message);
    }
}
