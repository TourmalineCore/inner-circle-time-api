using Xunit;

namespace Core.Features.Tracking.Entities;

[UnitTest]
public class TrackedEntryBaseTests
{
    [Fact]
    public void GetHours_WithFullHour_ShouldReturnCorrectHours()
    {
        var entry = new TrackedEntryBase
        {
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 18, 0, 0)
        };

        Assert.Equal(9m, entry.GetDurationInHours());
    }

    [Fact]
    public void GetHours_With30Minutes_ShouldReturnCorrectHours()
    {
        var entry = new TrackedEntryBase
        {
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 17, 30, 0)
        };

        Assert.Equal(8.5m, entry.GetDurationInHours());
    }


    [Fact]
    public void GetHours_With20Minutes_ShouldReturnCorrectHours()
    {
        var entry = new TrackedEntryBase
        {
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 20, 0)
        };
        Assert.Equal(1.3333333333333333333333333333m, entry.GetDurationInHours());
    }

    [Fact]
    public void GetTotalMinutes_ShouldReturnCorrectTotalMinutes()
    {
        var entry = new TrackedEntryBase
        {
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 12, 30, 0)
        };

        Assert.Equal(210, entry.GetDurationInMinutes());
    }

    [Fact]
    public void IsAllDay_WhenStartTimeAndEndTimeAreMidnight_ShouldReturnTrue()
    {
        var allDayEntry = new TrackedEntryBase
        {
            StartTime = new DateTime(2025, 11, 24, 0, 0, 0),
            EndTime = new DateTime(2025, 11, 25, 0, 0, 0)
        };

        Assert.True(allDayEntry.IsAllDay);
    }

    [Fact]
    public void IsAllDay_WhenStartTimeAndEndTimeAreSpecific_ShouldReturnFalse()
    {
        var nonAllDayEntry = new TrackedEntryBase
        {
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 18, 0, 0)
        };

        Assert.False(nonAllDayEntry.IsAllDay);
    }
}
