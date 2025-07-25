using Discord;
using Discord.WebSocket;
using DiscordTimeTracker.Domain.Enums;
using DiscordTimeTracker.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using DiscordTimeTracker.Application.UseCases.ClockIn;
using DiscordTimeTracker.Application.UseCases.ClockOut;
using DiscordTimeTracker.Application.UseCases.ManualEntry;
using DiscordTimeTracker.Infrastructure.Mongo;
using DiscordTimeTracker.Application.UseCases.GetEntriesOfToday;

internal class Program
{
    private static async Task Main(string[] args)
    {
        
        
        DotNetEnv.Env.Load();
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "";
        var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME") ?? "";
        var services = new ServiceCollection();
        services.AddSingleton<ITimeEntryRepository>(provider =>
        {
            return new MongoTimeEntryRepository(connectionString, databaseName);
        });
        services.AddTransient<ClockInUseCase>();
        services.AddTransient<ClockOutUseCase>();
        services.AddTransient<ManualEntryUseCase>();
        services.AddTransient<GetEntriesOfTodayUseCase>();
        services.AddSingleton<DiscordSocketClient>();

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<DiscordSocketClient>();

        client.Log += msg =>
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        };

        client.Ready += async () =>
        {
            Console.WriteLine($"Connected as -> [{client.CurrentUser}] 🟢");

            var globalCommandBuilder = new SlashCommandBuilder()
                .WithName("clockin")
                .WithDescription("Register your clock-in time");

            var clockOutBuilder = new SlashCommandBuilder()
                .WithName("clockout")
                .WithDescription("Register your clock-out time");

            var manualEntryBuilder = new SlashCommandBuilder()
                .WithName("manualentry")
                .WithDescription("Manually add a time entry")
                .AddOption("timestamp", ApplicationCommandOptionType.String, "Timestamp in UTC (yyyy-MM-ddTHH:mm:ss)", isRequired: true)
                .AddOption("type", ApplicationCommandOptionType.String, "Type: clockin or clockout", isRequired: true);

            var listEntriesBuilder = new SlashCommandBuilder()
                .WithName("entries")
                .WithDescription("List today's entries");

            await client.BulkOverwriteGlobalApplicationCommandsAsync(new[] {
        globalCommandBuilder.Build(),
        clockOutBuilder.Build(),
        manualEntryBuilder.Build(),
        listEntriesBuilder.Build()
            });
        };

        client.SlashCommandExecuted += async command =>
        {
            try
            {
                var userId = command.User.Id.ToString();
                var guildId = (command.GuildId ?? 0).ToString();
                var userName = command.User.Username;

                switch (command.CommandName)
                {
                    case "clockin":
                        var clockIn = provider.GetRequiredService<ClockInUseCase>();
                        await clockIn.ExecuteAsync(new ClockInRequest(userId, guildId, userName));
                        await command.RespondAsync($":white_check_mark: Clock-in registered for {userName}", ephemeral: true);
                        break;

                    case "clockout":
                        var clockOut = provider.GetRequiredService<ClockOutUseCase>();
                        await clockOut.ExecuteAsync(new ClockOutRequest(userId, guildId, userName));
                        await command.RespondAsync($":white_check_mark: Clock-out registered for {userName}", ephemeral: true);
                        break;

                    case "manualentry":
                        var timestampStr = command.Data.Options.First(x => x.Name == "timestamp").Value.ToString();
                        var typeStr = command.Data.Options.First(x => x.Name == "type").Value.ToString();

                        if (!DateTime.TryParse(timestampStr, out var parsedTime))
                        {
                            await command.RespondAsync($":warning: Invalid timestamp format.", ephemeral: true);
                            return;
                        }

                        var type = typeStr?.ToLower() == "clockin" ? TimeEntryType.ClockIn : TimeEntryType.ClockOut;
                        var manual = provider.GetRequiredService<ManualEntryUseCase>();
                        await manual.ExecuteAsync(new ManualEntryRequest(userId, guildId, userName, parsedTime, type));
                        await command.RespondAsync($":pencil: Manual entry added.", ephemeral: true);
                        break;

                    case "entries":
                        //await command.RespondAsync($":angry: O {userName} é um merda.", ephemeral: true);
                        //break;
                        await command.DeferAsync(ephemeral: true);
                        var entriesUseCase = provider.GetRequiredService<GetEntriesOfTodayUseCase>();
                        var entries = await entriesUseCase.ExecuteAsync(new GetEntriesOfTodayRequest(guildId, userId));

                        if (entries.Count == 0)
                        {
                            await command.FollowupAsync(":calendar: No entries found for today.", ephemeral: true);
                        }
                        else
                        {
                            var formatted = string.Join("\n", entries.Select(e => $"• `{e.Timestamp:HH:mm}` - {e.Type}"));
                            await command.FollowupAsync($":calendar: Entries for today:\n{formatted}", ephemeral: true);
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling command {command.CommandName}: {ex.Message}");
                try { await command.RespondAsync(":x: Error executing command.", ephemeral: true); } catch { }
            }
        };

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(-1);
    }
}