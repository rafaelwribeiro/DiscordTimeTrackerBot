namespace DiscordTimeTracker.Application.UseCases.ManualEntry;

public class ManualEntryResponse
{
    public string Message { get; }

    public ManualEntryResponse(string message)
    {
        Message = message;
    }
}
