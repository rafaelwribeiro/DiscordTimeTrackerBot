using Discord.WebSocket;
using DiscordTimeTracker.Application.UseCases.GetEntriesOfToday;

namespace DiscordTimeTracker.Bot.Handlers;

public class ListEntriesCommandHandler : ISlashCommandHandler
{
    private readonly GetEntriesOfTodayUseCase _useCase;

    public string CommandName => "entries";

    public ListEntriesCommandHandler(GetEntriesOfTodayUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task HandleAsync(SocketSlashCommand command)
    {
        await command.DeferAsync(ephemeral: true);
        var userId = command.User.Id.ToString();
        var guildId = (command.GuildId ?? 0).ToString();

        var result = await _useCase.ExecuteAsync(new GetEntriesOfTodayRequest(guildId, userId));
        if (result.IsFailure)
        {
            await command.FollowupAsync($":x: {result.Error}", ephemeral: true);
            return;
        }

        await command.FollowupAsync(result.Value?.Message ?? result.Error, ephemeral: true);
    }
}

