using Discord.WebSocket;
using DiscordTimeTracker.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using DiscordTimeTracker.Application.UseCases.ClockIn;
using DiscordTimeTracker.Application.UseCases.ClockOut;
using DiscordTimeTracker.Application.UseCases.ManualEntry;
using DiscordTimeTracker.Infrastructure.Mongo;
using DiscordTimeTracker.Application.UseCases.GetEntriesOfToday;
using DiscordTimeTracker.Bot.Handlers;
using DiscordTimeTracker.Bot;
using DiscordTimeTracker.Application.Interfaces;
using DiscordTimeTracker.Infrastructure.Reporting;
using DiscordTimeTracker.Application.UseCases.GenerateMonthlyReport;
using QuestPDF.Infrastructure;

internal class Program
{
    private static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load();

        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "";
        var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME") ?? "";

        QuestPDF.Settings.License = LicenseType.Community;

        var services = new ServiceCollection();
        services.AddSingleton<ITimeEntryRepository>(provider =>
        {
            return new MongoTimeEntryRepository(connectionString, databaseName);
        });

        services.AddTransient<ClockInUseCase>();
        services.AddTransient<ClockOutUseCase>();
        services.AddTransient<ManualEntryUseCase>();
        services.AddTransient<GetEntriesOfTodayUseCase>();        
        services.AddTransient<GenerateMonthlyReportUseCase>();        

        services.AddTransient<ISlashCommandHandler, ClockInCommandHandler>();
        services.AddTransient<ISlashCommandHandler, ClockOutCommandHandler>();
        services.AddTransient<ISlashCommandHandler, ManualEntryCommandHandler>();
        services.AddTransient<ISlashCommandHandler, ListEntriesCommandHandler>();
        services.AddTransient<ISlashCommandHandler, MonthlyReportCommandHandler>();

        services.AddScoped<IMonthlyReportPdfGenerator, QuestPdfMonthlyReportGenerator>();

        services.AddSingleton<SlashCommandDispatcher>();
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<DiscordBot>();


        var provider = services.BuildServiceProvider();

        var bot = provider.GetRequiredService<DiscordBot>();

        await bot.StartAsync(token!);

        await Task.Delay(-1);
    }
}