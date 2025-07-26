namespace DiscordTimeTracker.Application.Validators;

using DiscordTimeTracker.Domain.Entities;
using DiscordTimeTracker.Domain.Enums;

public class TimeEntryValidator
{
    public static bool CanClockIn(TimeEntry? lastEntry) =>
        lastEntry == null || lastEntry.Type == TimeEntryType.ClockOut;

    public static bool CanClockOut(TimeEntry? lastEntry) =>
        lastEntry != null && lastEntry.Type == TimeEntryType.ClockIn;

    public static bool CanManualEntry(TimeEntry? lastEntry, TimeEntryType newType)
    {
        if (lastEntry == null)
            return newType == TimeEntryType.ClockIn;

        if (lastEntry.Type == newType)
            return false;

        return true;
    }
}
