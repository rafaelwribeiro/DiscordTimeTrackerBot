namespace DiscordTimeTracker.Application.UseCases.ClockIn;

public class ClockInResponse
{
    public string Message { get; }

    public ClockInResponse(string message)
    {
        Message = message;
    }
}
