namespace DiscordTimeTracker.Application.UseCases.ClockOut;

public class ClockOutResponse
{
    public string Message { get; }

    public ClockOutResponse(string message)
    {
        Message = message;
    }
}
