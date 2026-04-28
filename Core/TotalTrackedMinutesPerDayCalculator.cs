using Core.Entities;

namespace Core;

public class TotalTrackedMinutesPerDayCalculator
{
    public static int Calculate(
        List<TrackedEntryBase> trackedEntries,
        DateTime startTime
    )
    {
        return trackedEntries
            .Where(x => x.StartTime.Date == startTime.Date)
            .Sum(x => x.StartTime.GetTotalMinutes(x.EndTime));
    }
}
