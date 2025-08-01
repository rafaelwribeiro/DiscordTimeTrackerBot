using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Repositories;
using MongoDB.Driver;

namespace DiscordTimeTracker.Infrastructure.Mongo;

public class MongoTimeEntryRepository : ITimeEntryRepository
{
    private readonly IMongoCollection<TimeEntry> _collection;

    public MongoTimeEntryRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<TimeEntry>("time_entries");

        CreateIndexesIfNeeded(_collection);
    }

    public async Task AddAsync(TimeEntry entry)
    {
        await _collection.InsertOneAsync(entry);
    }

    public async Task<List<TimeEntry>> GetByUserAsync(string guildId, string userId, DateTime? from = null, DateTime? to = null)
    {
        var builder = Builders<TimeEntry>.Filter;
        var filter = builder.Eq(x => x.GuildId, guildId) & builder.Eq(x => x.UserId, userId);

        if (from.HasValue)
            filter &= builder.Gte(x => x.Timestamp, from.Value);

        if (to.HasValue)
            filter &= builder.Lte(x => x.Timestamp, to.Value);

        return await _collection.Find(filter).SortBy(x => x.Timestamp).ToListAsync();
    }

    private void CreateIndexesIfNeeded(IMongoCollection<TimeEntry> collection)
    {
        var indexKeys = Builders<TimeEntry>.IndexKeys
            .Ascending(e => e.GuildId)
            .Ascending(e => e.UserId)
            .Ascending(e => e.Timestamp);

        var indexModel = new CreateIndexModel<TimeEntry>(indexKeys);
        collection.Indexes.CreateOne(indexModel);
    }

    public async Task<IReadOnlyList<TimeEntry>> GetEntriesByUserAndGuildAndDateRangeAsync(string guildId, string userId, DateTime start, DateTime end)
    {
        Console.WriteLine($"Start: {start:O}");
        Console.WriteLine($"End: {end:O}");
        Console.WriteLine($"guildId: {guildId}");
        Console.WriteLine($"userId: {userId}");
        var result = await _collection
        .Find(x => x.GuildId == guildId
                   && x.UserId == userId
                   && x.Timestamp >= start
                   && x.Timestamp <= end)
        .ToListAsync();

        return result;
    }

    public async Task<TimeEntry?> GetLastEntryByUserAsync(string guildId, string userId)
    {
        var filter = Builders<TimeEntry>.Filter.And(
            Builders<TimeEntry>.Filter.Eq(x => x.GuildId, guildId),
            Builders<TimeEntry>.Filter.Eq(x => x.UserId, userId)
        );

        var sort = Builders<TimeEntry>.Sort.Descending(x => x.Timestamp);

        var result = await _collection.Find(filter)
                                      .Sort(sort)
                                      .Limit(1)
                                      .FirstOrDefaultAsync();

        return result;
    }

    public async Task<List<TimeEntry>> GetEntriesByUserAndGuildAndMonthAsync(string guildId, string userId, int year, int month)
    {
        var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1).AddTicks(-1);

        var filter = Builders<TimeEntry>.Filter.And(
            Builders<TimeEntry>.Filter.Eq(e => e.GuildId, guildId),
            Builders<TimeEntry>.Filter.Eq(e => e.UserId, userId),
            Builders<TimeEntry>.Filter.Gte(e => e.Timestamp, start),
            Builders<TimeEntry>.Filter.Lte(e => e.Timestamp, end)
        );

        return await _collection.Find(filter).ToListAsync();
    }
}
