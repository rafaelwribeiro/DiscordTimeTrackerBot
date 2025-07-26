namespace DiscordTimeTracker.Application.UseCases.GetEntriesOfToday;

public class GetEntriesOfTodayResponse
{
    public string Message { get; }

    public GetEntriesOfTodayResponse(string message)
    {
        Message = message;
    }
}
