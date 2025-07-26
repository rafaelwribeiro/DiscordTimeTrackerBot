using DiscordTimeTracker.Application.UseCases.ClockIn;
using DiscordTimeTracker.Application.UseCases.ClockOut;
using DiscordTimeTracker.Application.UseCases.GetEntriesOfToday;
using DiscordTimeTracker.Application.UseCases.ManualEntry;
using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Enums;
using DiscordTimeTracker.Infrastructure.Mongo;
using DotNetEnv;
using System.Globalization;

static class Program
{
    private static MongoTimeEntryRepository _repo;
    private static string _guildId;
    private static string _userId;
    private static string _userName;

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Carrega variáveis do .env
        Env.Load();

        _guildId = Environment.GetEnvironmentVariable("GUILD_ID")!;
        _userId = Environment.GetEnvironmentVariable("USER_ID")!;
        _userName = Environment.GetEnvironmentVariable("USER_NAME")!;

        var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION")!;
        var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE")!;

        _repo = new MongoTimeEntryRepository(connectionString, databaseName);

        Console.WriteLine("Welcome to Madruga Time Tracker Console");

        while (true)
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1 - Clock In");
            Console.WriteLine("2 - Clock Out");
            Console.WriteLine("3 - Manual Entry");
            Console.WriteLine("4 - List Entries");
            Console.WriteLine("0 - Exit");

            Console.Write("Option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await ClockIn();
                    break;
                case "2":
                    await ClockOut();
                    break;
                case "3":
                    await ManualEntry();
                    break;
                case "4":
                    await ListEntries();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    private static async Task ClockIn()
    {
        var request = new ClockInRequest(_guildId, _userId, _userName);
        var useCase = new ClockInUseCase(_repo);

        var result = await useCase.ExecuteAsync(request);

        if (result.IsFailure)
        {
            Console.WriteLine($"❌ {result.Error}");
            return;
        }

        Console.WriteLine($"✅ {result.Value.Message}");
    }

    private static async Task ClockOut()
    {
        var request = new ClockOutRequest(_guildId, _userId, _userName);
        var useCase = new ClockOutUseCase(_repo);

        var result = await useCase.ExecuteAsync(request);

        if (result.IsFailure)
        {
            Console.WriteLine($"❌ {result.Error}");
            return;
        }

        Console.WriteLine($"✅ {result.Value.Message}");
    }

    private static async Task ManualEntry()
    {
        Console.Write("Enter type (0 - ClockIn, 1 - ClockOut): ");
        var typeStr = Console.ReadLine();

        if (!Enum.TryParse<TimeEntryType>(typeStr, out var type))
        {
            Console.WriteLine("Invalid type.");
            return;
        }

        Console.Write("Enter timestamp (yyyy-MM-dd HH:mm:ss): ");
        var timestampStr = Console.ReadLine();

        if (!DateTime.TryParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var timestamp))
        {
            Console.WriteLine("Invalid timestamp format.");
            return;
        }

        var useCase = new ManualEntryUseCase(_repo);
        var request = new ManualEntryRequest(
            guildId: _guildId,
            userId: _userId,
            userName: _userName,
            timestamp: timestamp.ToUniversalTime(),
            type: type            
        );

        var result = await useCase.ExecuteAsync(request);

        if (result.IsFailure)
        {
            Console.WriteLine($"❌ {result.Error}");
            return;
        }

        Console.WriteLine($"✅ {result.Value.Message}");
    }

    private static async Task ListEntries()
    {
        var useCase = new GetEntriesOfTodayUseCase(_repo);
        var request = new GetEntriesOfTodayRequest(_guildId, _userId);

        var result = await useCase.ExecuteAsync(request);

        if (result.IsFailure)
        {
            Console.WriteLine($"❌ {result.Error}");
            return;
        }

        Console.WriteLine($"✅ {result.Value.Message}");
    }
}
