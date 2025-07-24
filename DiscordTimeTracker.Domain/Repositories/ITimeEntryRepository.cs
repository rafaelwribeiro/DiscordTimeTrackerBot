using DiscordTimeTracker.Domain.Entities;

namespace DiscordTimeTracker.Domain.Repositories;

public interface ITimeEntryRepository
{
    Task AddAsync(TimeEntry entry);

    Task<List<TimeEntry>> GetByUserAsync(ulong guildId, ulong userId, DateTime? from = null, DateTime? to = null);
}
