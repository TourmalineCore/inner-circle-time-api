using Core.Entities;

namespace Core;

public class TotalTrackedHoursPerDayCalculator
{
    public static decimal Calculate(
        List<TrackedEntryBase> trackedEntries,
        DateTime startTime
    )
    {
        return (decimal)trackedEntries
            .Where(x => x.StartTime.Date == startTime.Date)
            .Sum(x => x.Duration.TotalHours);
    }
}
