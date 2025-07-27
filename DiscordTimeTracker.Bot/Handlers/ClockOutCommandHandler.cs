using Discord.WebSocket;
using DiscordTimeTracker.Application.UseCases.ClockOut;

namespace DiscordTimeTracker.Bot.Handlers;

public class ClockOutCommandHandler : ISlashCommandHandler
{
    private readonly ClockOutUseCase _useCase;

    public string CommandName => "clockout";

    public ClockOutCommandHandler(ClockOutUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task HandleAsync(SocketSlashCommand command)
    {
        var userId = command.User.Id.ToString();
        var guildId = (command.GuildId ?? 0).ToString();
        var userName = command.User.Username;

        var result = await _useCase.ExecuteAsync(new ClockOutRequest(guildId, userId, userName));
        await command.RespondAsync(result.Value?.Message ?? result.Error, ephemeral: true);
    }
}
