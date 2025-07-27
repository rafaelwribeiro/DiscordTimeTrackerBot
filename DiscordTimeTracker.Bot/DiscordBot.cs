using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using DiscordTimeTracker.Bot.Handlers;

namespace DiscordTimeTracker.Bot;

public class DiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly SlashCommandDispatcher _dispatcher;

    public DiscordBot(DiscordSocketClient client, SlashCommandDispatcher executor)
    {
        _client = client;
        _dispatcher = executor;
    }

    public async Task StartAsync(string token)
    {
        _client.Log += msg =>
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        };

        _client.Ready += async () =>
        {
            Console.WriteLine($"Connected as [{_client.CurrentUser}]");

            var commands = new[]
            {
                new SlashCommandBuilder()
                    .WithName("clockin")
                    .WithDescription("Register your clock-in time").Build(),

                new SlashCommandBuilder()
                    .WithName("clockout")
                    .WithDescription("Register your clock-out time").Build(),

                new SlashCommandBuilder()
                    .WithName("manualentry")
                    .WithDescription("Manually add a time entry")
                    .AddOption("timestamp", ApplicationCommandOptionType.String, "Timestamp in UTC (yyyy-MM-ddTHH:mm:ss)", true)
                    .AddOption("type", ApplicationCommandOptionType.String, "Type: clockin or clockout", true).Build(),

                new SlashCommandBuilder()
                    .WithName("entries")
                    .WithDescription("List today's entries").Build()
            };

            await _client.BulkOverwriteGlobalApplicationCommandsAsync(commands);
        };

        _client.SlashCommandExecuted += async command =>
        {
            try
            {
                await _dispatcher.DispatchAsync(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling command {command.CommandName}: {ex.Message}");
                try { await command.RespondAsync(":x: Error executing command.", ephemeral: true); } catch { }
            }
        };

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }
}
