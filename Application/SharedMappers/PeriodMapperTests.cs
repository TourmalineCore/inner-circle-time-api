using Core;
using Xunit;

namespace Application.SharedMappers;

[UnitTest]
public class PeriodMapperTests
{
    [Fact]
    public void MapToPeriodDto_ShouldReturnCorrectPeriodDtoWithSubtractOneDayFromEndDate()
    {
        var startTime = new DateTime(2026, 7, 20, 0, 0, 0);
        var endTime = new DateTime(2026, 7, 25, 0, 0, 0);

        var period = PeriodMapper.MapToPeriodDto(startTime, endTime);

        Assert.Equal(new DateOnly(2026, 7, 20), period.StartDate);
        Assert.Equal(new DateOnly(2026, 7, 24), period.EndDate);
    }
}
