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

        Console.WriteLine("Bem-vindo ao Madruga Time Tracker Console");

        while (true)
        {
            Console.WriteLine("\nSelecione uma opção:");
            Console.WriteLine("1 - Clock In");
            Console.WriteLine("2 - Clock Out");
            Console.WriteLine("3 - Entrada Manual");
            Console.WriteLine("4 - Listar Entradas");
            Console.WriteLine("0 - Sair");

            Console.Write("Opção: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await Clock(TimeEntryType.ClockIn);
                    break;
                case "2":
                    await Clock(TimeEntryType.ClockOut);
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

    private static async Task Clock(TimeEntryType type)
    {
        var entry = CreateBasicEntry();
        entry.Type = type;
        await _repo.AddAsync(entry);
        Console.WriteLine($"{type} registrado em {entry.Timestamp:HH:mm:ss}");
    }

    private static async Task ManualEntry()
    {
        var entry = CreateBasicEntry();

        Console.Write("Digite o tipo (0 - ClockIn, 1 - ClockOut): ");
        var typeStr = Console.ReadLine();

        if (!Enum.TryParse<TimeEntryType>(typeStr, out var type))
        {
            Console.WriteLine("Tipo inválido.");
            return;
        }

        Console.Write("Digite a data e hora (yyyy-MM-dd HH:mm:ss): ");
        var timestampStr = Console.ReadLine();

        if (!DateTime.TryParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var timestamp))
        {
            Console.WriteLine("Data/hora inválida.");
            return;
        }

        entry.Type = type;
        entry.Timestamp = timestamp.ToUniversalTime();

        await _repo.AddAsync(entry);
        Console.WriteLine("Entrada manual registrada.");
    }

    private static async Task ListEntries()
    {
        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(1).AddTicks(-1);

        var entries = await _repo.GetEntriesByUserAndGuildAndDateRangeAsync(_guildId, _userId, start, end);

        Console.WriteLine($"\nEntradas para {_userName} em {start:yyyy-MM-dd}");
        foreach (var e in entries.OrderBy(e => e.Timestamp))
        {
            Console.WriteLine($"[{e.Timestamp:HH:mm:ss}] {e.Type}");
        }
    }

    private static TimeEntry CreateBasicEntry()
    {
        return new TimeEntry
        {
            GuildId = _guildId,
            UserId = _userId,
            UserName = _userName
        };
    }
}
