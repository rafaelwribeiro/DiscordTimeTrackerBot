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
        var result = await _collection
        .Find(x => x.GuildId == guildId
                   && x.UserId == userId
                   && x.Timestamp >= start
                   && x.Timestamp <= end)
        .ToListAsync();

        return result;
    }


}
