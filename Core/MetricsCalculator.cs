using Core.Entities;

namespace Core;

public class Metrics
{
    public required decimal TrackedHours { get; set; }
}

public class MetricsCalculator
{
    public static Metrics Calculate(List<TrackedEntryBase> trackedEntries)
    {
        var taskTotalMinutes = trackedEntries
             .OfType<TaskEntry>()
             .Sum(x => x.GetDurationInMinutes());

        var unwellTotalMinutes = trackedEntries
            .OfType<UnwellEntry>()
            .Sum(x => x.GetDurationInMinutes());

        var trackedHours = (taskTotalMinutes + unwellTotalMinutes).ToHoursWithoutRounding();

        return new Metrics
        {
            TrackedHours = trackedHours
        };
    }
}
