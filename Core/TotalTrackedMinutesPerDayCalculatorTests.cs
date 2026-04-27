using Core.Entities;
using Xunit;

namespace Core;

[UnitTest]
public class TotalTrackedMinutesPerDayCalculatorTests
{
    [Fact]
    public void TotalTrackedMinutesPerDayCalculatorWithoutTrackedEntries_ShouldReturnZero()
    {
        var trackedEntries = new List<TrackedEntryBase> { };

        var calculatedTrackedMinutesPerDay = TotalTrackedMinutesPerDayCalculator.Calculate(trackedEntries, new DateTime(2025, 11, 20));

        Assert.Equal(0, calculatedTrackedMinutesPerDay);
    }

    [Fact]
    public void TotalTrackedMinutesPerDayCalculatorWithDateWithoutTrackedEntries_ShouldReturnZero()
    {
        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 12, 0, 0)
            },
        };

        var calculatedTrackedMinutesPerDay = TotalTrackedMinutesPerDayCalculator.Calculate(trackedEntries, new DateTime(1999, 11, 24));

        Assert.Equal(0, calculatedTrackedMinutesPerDay);
    }

    [Fact]
    public void TotalTrackedMinutesPerDayCalculatorWithOneTrackedEntry_ShouldReturnCorrectTrackedMinutesPerDay()
    {
        var startTime = new DateTime(2025, 11, 24, 9, 0, 0);

        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = startTime,
                EndTime = new DateTime(2025, 11, 24, 12, 0, 0)
            },
        };

        var calculatedTrackedMinutesPerDay = TotalTrackedMinutesPerDayCalculator.Calculate(trackedEntries, startTime);

        Assert.Equal(180, calculatedTrackedMinutesPerDay);
    }

    [Fact]
    public void TotalTrackedMinutesPerDayCalculatorWithTwoTrackedEntries_ShouldReturnCorrectTrackedMinutesPerDay()
    {
        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 11, 0, 0)
            },
            new UnwellEntry
            {
                StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 16, 30, 0)
            },
        };

        var calculatedTrackedMinutesPerDay = TotalTrackedMinutesPerDayCalculator.Calculate(trackedEntries, new DateTime(2025, 11, 24));

        Assert.Equal(330, calculatedTrackedMinutesPerDay);
    }

    [Fact]
    public void TotalTrackedMinutesPerDayCalculatorWithTwoTrackedEntriesWithDifferentDate_ShouldReturnCorrectTrackedMinutesPerDay()
    {
        var firstEntryStartTime = new DateTime(2025, 11, 01, 9, 0, 0);

        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = firstEntryStartTime,
                EndTime = new DateTime(2025, 11, 01, 11, 0, 0)
            },
            new UnwellEntry
            {
                StartTime = new DateTime(2025, 11, 30, 9, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 11, 0, 0)
            },
        };

        var calculatedTrackedMinutesPerDay = TotalTrackedMinutesPerDayCalculator.Calculate(trackedEntries, firstEntryStartTime);

        Assert.Equal(120, calculatedTrackedMinutesPerDay);
    }
}
