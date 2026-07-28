using Core;
using Xunit;


[UnitTest]
public class PeriodToDurationConverterTests
{
    [Fact]
    public void ConvertToDuration_ShouldReturnDurationThatStartsOnMidnightOfStartDateAndEndsOnMidnightOfTheDayAfterEndDate()
    {
        var period = new Period
        {
            StartDate = new DateOnly(2026, 7, 13),
            EndDate = new DateOnly(2026, 7, 17)
        };

        var result = PeriodToDurationConverter.ConvertToDuration(period);

        Assert.Equal(new DateTime(2026, 7, 13, 0, 0, 0), result.StartTime);
        Assert.Equal(new DateTime(2026, 7, 18, 0, 0, 0), result.EndTime);
    }
}
