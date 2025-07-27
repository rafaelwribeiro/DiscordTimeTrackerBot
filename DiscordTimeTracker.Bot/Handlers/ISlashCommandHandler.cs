using Discord.WebSocket;

namespace DiscordTimeTracker.Bot.Handlers;

public interface ISlashCommandHandler
{
    string CommandName { get; }
    Task HandleAsync(SocketSlashCommand command);
}

