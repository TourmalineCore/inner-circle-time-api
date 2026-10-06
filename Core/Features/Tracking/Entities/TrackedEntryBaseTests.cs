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
    public void StartTimeAndEndTimeSetters_ShouldResetSecondsToZero()
    {
        var entry = new TrackedEntryBase
        {
            StartTime = new DateTime(2025, 11, 24, 7, 0, 10),
            EndTime = new DateTime(2025, 11, 24, 12, 0, 30)
        };

        Assert.Equal(new DateTime(2025, 11, 24, 7, 0, 0), entry.StartTime);
        Assert.Equal(new DateTime(2025, 11, 24, 12, 0, 0), entry.EndTime);
    }
}
