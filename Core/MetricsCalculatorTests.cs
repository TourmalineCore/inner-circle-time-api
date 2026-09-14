using Core.Entities;
using Xunit;

namespace Core;

[UnitTest]
public class MetricsCalculatorTests
{
    public static IEnumerable<object[]> TrackedEntriesData()
    {
        // Empty list
        yield return new object[]
        {
            new List<TrackedEntryBase> { },
            0m
        };

        // Only Task
        yield return new object[]
        {
            new List<TrackedEntryBase>
            {
                new TaskEntry
                {
                    StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 9, 30, 0),
                }
            },
            0.5m
        };

        // Only Unwell
        yield return new object[]
        {
            new List<TrackedEntryBase>
            {
                new UnwellEntry
                {
                    StartTime = new DateTime(2025, 11, 24, 10, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 11, 20, 0),
                }
            },
            1.3333333333333333333333333333m
        };

        // Unwell and Task
        yield return new object[]
        {
            new List<TrackedEntryBase>
            {
                new UnwellEntry
                {
                    StartTime = new DateTime(2025, 11, 24, 10, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 11, 20, 0),
                },
                new TaskEntry
                {
                    StartTime = new DateTime(2025, 11, 24, 11, 20, 0),
                    EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
                }
            },
            2m
        };

        // Unwell and two Task
        yield return new object[]
        {
            new List<TrackedEntryBase>
            {
                new UnwellEntry
                {
                    StartTime = new DateTime(2025, 11, 24, 8, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 8, 20, 0),
                },
                new TaskEntry
                {
                    StartTime = new DateTime(2025, 11, 24, 8, 20, 0),
                    EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
                },
                new TaskEntry
                {
                    StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 17, 0, 0),
                }
            },
            8m
        };
    }

    [Theory]
    [MemberData(nameof(TrackedEntriesData))]
    public void Calculate_ShouldReturnExpectedTrackedHours(
       List<TrackedEntryBase> trackedEntries,
       decimal expectedTrackedHours
    )
    {
        var metrics = MetricsCalculator.Calculate(trackedEntries);

        Assert.Equal(expectedTrackedHours, metrics.TrackedHours);
    }
}
