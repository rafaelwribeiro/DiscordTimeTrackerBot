using Discord;
using Discord.WebSocket;
using DiscordTimeTracker.Application.UseCases.GenerateMonthlyReport;

namespace DiscordTimeTracker.Bot.Handlers;

public class MonthlyReportCommandHandler : ISlashCommandHandler
{
    private readonly GenerateMonthlyReportUseCase _useCase;

    public MonthlyReportCommandHandler(GenerateMonthlyReportUseCase useCase)
    {
        _useCase = useCase;
    }

    public string CommandName => "monthlyreport";

    public async Task HandleAsync(SocketSlashCommand command)
    {
        var userId = command.User.Id.ToString();
        var guildId = (command.GuildId ?? 0).ToString();
        string userName = GetUserName(command);

        var month = Convert.ToInt32(command.Data.Options.FirstOrDefault(x => x.Name == "month")?.Value);
        var year = Convert.ToInt32(command.Data.Options.FirstOrDefault(x => x.Name == "year")?.Value);

        var result = await _useCase.ExecuteAsync(new GenerateMonthlyReportRequest(guildId, userId, userName, year, month));

        if (result.IsFailure)
        {
            await command.RespondAsync($":x: {result.Error}", ephemeral: true);
            return;
        }

        var report = result.Value!;

        var fileName = report.FileName;

        // Create in-memory PDF stream
        var stream = new MemoryStream(report.FileBytes);
        stream.Position = 0;

        var attachment = new FileAttachment(stream, fileName);

        await command.RespondWithFileAsync(attachment, text: $"📄 Monthly Report for **{month:D2}/{year}**", ephemeral: true);
    }

    private static string GetUserName(SocketSlashCommand command)
    {
        var user = command.User as SocketGuildUser;
        var displayName = user?.Nickname ?? user?.Username ?? command.User.Username;
        
        return displayName;
    }
}
