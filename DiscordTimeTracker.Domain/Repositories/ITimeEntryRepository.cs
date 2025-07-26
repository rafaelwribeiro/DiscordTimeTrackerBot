using DiscordTimeTracker.Domain.Entities;

namespace DiscordTimeTracker.Domain.Repositories;

public interface ITimeEntryRepository
{
    Task AddAsync(TimeEntry entry);

    Task<List<TimeEntry>> GetByUserAsync(string guildId, string userId, DateTime? from = null, DateTime? to = null);
    Task<TimeEntry?> GetLastEntryByUserAsync(string guildId, string userId);
    Task<IReadOnlyList<TimeEntry>> GetEntriesByUserAndGuildAndDateRangeAsync(string guildId, string userId, DateTime start, DateTime end);
    Task<List<TimeEntry>> GetEntriesByUserAndGuildAndMonthAsync(string guildId, string userId, int year, int month);

}
