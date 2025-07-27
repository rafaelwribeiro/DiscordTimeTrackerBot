using Discord.WebSocket;
using DiscordTimeTracker.Application.UseCases.ClockIn;

namespace DiscordTimeTracker.Bot.Handlers;

public class ClockInCommandHandler : ISlashCommandHandler
{
    private readonly ClockInUseCase _useCase;

    public string CommandName => "clockin";

    public ClockInCommandHandler(ClockInUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task HandleAsync(SocketSlashCommand command)
    {
        var userId = command.User.Id.ToString();
        var guildId = (command.GuildId ?? 0).ToString();
        var userName = command.User.Username;

        var result = await _useCase.ExecuteAsync(new ClockInRequest(guildId, userId, userName));
        await command.RespondAsync(result.Value?.Message ?? result.Error, ephemeral: true);
    }
}
