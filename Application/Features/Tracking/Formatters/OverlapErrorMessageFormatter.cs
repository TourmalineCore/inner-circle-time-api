using Core.Features.Tracking.Entities;

namespace Application.Features.Tracking.Formatters;

public static class OverlapErrorMessageFormatter
{
    public static string GetErrorMessage(TrackedEntryBase newEntry, TrackedEntryBase conflictingEntry)
    {
        if (conflictingEntry.IsAllDay)
        {
            return $"{GetDisplayName(newEntry.Type)} cannot be tracked on the day of {GetDisplayName(conflictingEntry.Type).ToLower()}. Adjust the time or edit the existing entry.";
        }

        return $"{GetDisplayName(conflictingEntry.Type)} is already tracked from {conflictingEntry.StartTime:HH:mm}-{conflictingEntry.EndTime:HH:mm}. Adjust the time or edit the existing entry.";
    }

    private static string GetDisplayName(EntryType entryType)
    {
        return entryType switch
        {
            EntryType.Task => "Task",
            EntryType.Unwell => "Unwell",
            EntryType.AwayWithMakeUpTime => "Away with make-up time",
            EntryType.MakeUpTime => "Make-up time",
            EntryType.SickLeave => "Sick leave",
            EntryType.Vacation => "Vacation",
            _ => throw new InvalidOperationException(
                $"Missing display name mapping for EntryType '{entryType}'. " +
                $"Please add '{entryType}' to OverlapErrorMessageFormatter.GetDisplayName.")
        };
    }
}
