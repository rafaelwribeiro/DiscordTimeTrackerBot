using Discord.WebSocket;
using DiscordTimeTracker.Application.UseCases.ClockIn;
using DiscordTimeTracker.Bot.Handlers;

namespace DiscordTimeTracker.Bot;

public class SlashCommandDispatcher
{
    private readonly Dictionary<string, ISlashCommandHandler> _handlers;

    public SlashCommandDispatcher(IEnumerable<ISlashCommandHandler> handlers)
    {
        _handlers = handlers.ToDictionary(h => h.CommandName, StringComparer.OrdinalIgnoreCase);
    }

    public async Task DispatchAsync(SocketSlashCommand command)
    {
        if (_handlers.TryGetValue(command.CommandName, out var handler))
        {
            await handler.HandleAsync(command);
        }
        else
        {
            await command.RespondAsync(":x: Unknown command", ephemeral: true);
        }
    }
}