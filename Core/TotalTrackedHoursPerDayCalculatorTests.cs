using Core.Entities;
using Xunit;

namespace Core;

[UnitTest]
public class TotalTrackedHoursPerDayCalculatorTests
{
    [Fact]
    public void TotalTrackedHoursPerDayCalculatorWithoutTrackedEntries_ShouldReturnZero()
    {
        var trackedEntries = new List<TrackedEntryBase> { };

        var calculatedTrackedHoursPerDay = TotalTrackedHoursPerDayCalculator.Calculate(trackedEntries, new DateTime(2025, 11, 20));

        Assert.Equal(0, calculatedTrackedHoursPerDay);
    }

    [Fact]
    public void TotalTrackedHoursPerDayCalculatorWithDateWithoutTrackedEntries_ShouldReturnZero()
    {
        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = new DateTime(2025, 11, 24),
                Duration = TimeSpan.Parse("03:00:00")
            },
        };

        var calculatedTrackedHoursPerDay = TotalTrackedHoursPerDayCalculator.Calculate(trackedEntries, new DateTime(1999, 11, 24));

        Assert.Equal(0, calculatedTrackedHoursPerDay);
    }

    [Fact]
    public void TotalTrackedHoursPerDayCalculatorWithOneTrackedEntry_ShouldReturnCorrectTrackedHoursPerDay()
    {
        var startTime = new DateTime(2025, 11, 24);

        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = startTime,
                Duration = TimeSpan.Parse("03:00:00")
            },
        };

        var calculatedTrackedHoursPerDay = TotalTrackedHoursPerDayCalculator.Calculate(trackedEntries, startTime);

        Assert.Equal(3, calculatedTrackedHoursPerDay);
    }

    [Fact]
    public void TotalTrackedHoursPerDayCalculatorWithTwoTrackedEntries_ShouldReturnCorrectTrackedHoursPerDay()
    {
        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = new DateTime(2025, 11, 24),
                Duration = TimeSpan.Parse("02:00:00")
            },
            new UnwellEntry
            {
                StartTime = new DateTime(2025, 11, 24),
                Duration = TimeSpan.Parse("03:30:00")
            },
        };

        var calculatedTrackedHoursPerDay = TotalTrackedHoursPerDayCalculator.Calculate(trackedEntries, new DateTime(2025, 11, 24));

        Assert.Equal(5.5m, calculatedTrackedHoursPerDay);
    }

    [Fact]
    public void TotalTrackedHoursPerDayCalculatorWithTwoTrackedEntriesWithDifferentDate_ShouldReturnCorrectTrackedHoursPerDay()
    {
        var firstEntryStartTime = new DateTime(2025, 11, 01);

        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = firstEntryStartTime,
                Duration = TimeSpan.Parse("02:00:00")
            },
            new UnwellEntry
            {
                StartTime = new DateTime(2025, 11, 30),
                Duration = TimeSpan.Parse("03:00:00")
            },
        };

        var calculatedTrackedHoursPerDay = TotalTrackedHoursPerDayCalculator.Calculate(trackedEntries, firstEntryStartTime);

        Assert.Equal(2, calculatedTrackedHoursPerDay);
    }

    [Fact]
    public void TotalTrackedHoursPerDayCalculatorWithIncompleteHours_ShouldReturnCorrectTrackedHoursPerDay()
    {
        var startTime = new DateTime(2025, 11, 24);

        var trackedEntries = new List<TrackedEntryBase> {
            new TaskEntry
            {
                StartTime = startTime,
                Duration = TimeSpan.Parse("08:00:00")
            },
            new TaskEntry {
                StartTime = startTime,
                Duration = TimeSpan.Parse("00:20:00")
            },
            new TaskEntry {
                StartTime = startTime,
                Duration = TimeSpan.Parse("00:39:00")
            },
            new TaskEntry {
                StartTime = startTime,
                Duration = TimeSpan.Parse("00:01:00")
            },
            new TaskEntry {
                StartTime = startTime,
                Duration = TimeSpan.Parse("00:48:00")
            },
            new TaskEntry {
                StartTime = startTime,
                Duration = TimeSpan.Parse("00:12:00")
            },
        };

        var calculatedTrackedHoursPerDay = TotalTrackedHoursPerDayCalculator.Calculate(trackedEntries, startTime);

        Assert.Equal(10, calculatedTrackedHoursPerDay);
    }
}
