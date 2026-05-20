using Core.Entities;

namespace Core;

public class TotalTrackedMinutesPerDayCalculator
{
    public static decimal Calculate(
        List<TrackedEntryBase> trackedEntries,
        DateTime startTime
    )
    {
        return trackedEntries
            .Where(x => x.StartTime.Date == startTime.Date)
            .Sum(x => x.GetDurationInMinutes());
    }
}
