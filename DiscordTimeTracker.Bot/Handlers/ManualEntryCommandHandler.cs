using Discord.WebSocket;
using DiscordTimeTracker.Application.UseCases.ManualEntry;
using global::DiscordTimeTracker.Domain.Enums;

namespace DiscordTimeTracker.Bot.Handlers;

public class ManualEntryCommandHandler : ISlashCommandHandler
{
    private readonly ManualEntryUseCase _useCase;

    public string CommandName => "manualentry";

    public ManualEntryCommandHandler(ManualEntryUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task HandleAsync(SocketSlashCommand command)
    {
        var timestampStr = command.Data.Options.FirstOrDefault(x => x.Name == "timestamp")?.Value?.ToString();
        var typeStr = command.Data.Options.FirstOrDefault(x => x.Name == "type")?.Value?.ToString();

        if (!DateTime.TryParse(timestampStr, out var timestamp))
        {
            await command.RespondAsync(":x: Invalid timestamp. Format must be `yyyy-MM-ddTHH:mm:ss` (UTC).", ephemeral: true);
            return;
        }

        if (!Enum.TryParse<TimeEntryType>(typeStr, true, out var type))
        {
            await command.RespondAsync(":x: Invalid type. Must be `clockin` or `clockout`.", ephemeral: true);
            return;
        }

        var userId = command.User.Id.ToString();
        var guildId = (command.GuildId ?? 0).ToString();
        var userName = command.User.Username;

        var result = await _useCase.ExecuteAsync(new ManualEntryRequest(guildId, userId, userName, timestamp, type));
        await command.RespondAsync(result.Value?.Message ?? result.Error, ephemeral: true);
    }
}