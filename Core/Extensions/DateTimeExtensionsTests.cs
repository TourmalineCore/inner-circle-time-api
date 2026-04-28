using Xunit;

namespace Core.Extensions;

[UnitTest]
public class DateTimeExtensionsTests
{
    [Fact]
    public void GetHours_WithFullHour_ShouldReturnCorrectHours()
    {
        var startTime = new DateTime(2025, 11, 24, 9, 0, 0);
        var endTime = new DateTime(2025, 11, 24, 18, 0, 0);

        Assert.Equal(9m, startTime.GetHours(endTime));
    }

    [Fact]
    public void GetHours_With30Minutes_ShouldReturnCorrectHours()
    {
        var startTime = new DateTime(2025, 11, 24, 9, 0, 0);
        var endTime = new DateTime(2025, 11, 24, 17, 30, 0);

        Assert.Equal(8.5m, startTime.GetHours(endTime));
    }

    [Fact]
    public void GetHours_With20Minutes_ShouldReturnCorrectHours()
    {
        var startTime = new DateTime(2025, 11, 24, 9, 0, 0);
        var endTime = new DateTime(2025, 11, 24, 10, 20, 0);

        Assert.Equal(1.3333333333333333333333333333m, startTime.GetHours(endTime));
    }

    [Fact]
    public void GetTotalMinutes_ShouldReturnCorrectTotalMinutes()
    {
        var startTime = new DateTime(2025, 11, 24, 9, 0, 0);
        var endTime = new DateTime(2025, 11, 24, 12, 30, 0);

        Assert.Equal(210, startTime.GetTotalMinutes(endTime));
    }
}
