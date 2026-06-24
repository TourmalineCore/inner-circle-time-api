using Core;
using Xunit;

namespace Application.Validators;

[UnitTest]
public class MakeUpTimeValidatorTests
{
    [Fact]
    public async Task MakeUpTimeValidator_ShouldReturnTrueIfMakeUpTotalTimeIsConveringWithPeriod()
    {
        var startTime = new DateTime(2025, 11, 24, 10, 0, 0);
        var endTime = new DateTime(2025, 11, 24, 11, 30, 0);

        var makeUpTimeList = new List<MakeUpTimeEntryDto>
        {
            new MakeUpTimeEntryDto
            {
                StartTime = new DateTime(2025, 11, 24, 15, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 16, 0, 0),
            },
            new MakeUpTimeEntryDto
            {
                StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 13, 30, 0),
            }
        };

        var result = MakeUpTimeValidator.IsMakeUpTotalTimeConvergingWithPeriod(startTime, endTime, makeUpTimeList);

        Assert.True(result);
    }

    [Fact]
    public async Task MakeUpTimeValidator_ShouldReturnFalseIfMakeUpTotalTimeIsNotConveringWithPeriod()
    {
        var startTime = new DateTime(2025, 11, 24, 10, 0, 0);
        var endTime = new DateTime(2025, 11, 24, 11, 30, 0);

        var makeUpTimeList = new List<MakeUpTimeEntryDto>
        {
            new MakeUpTimeEntryDto
            {
                StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 16, 30, 0),
            }
        };

        var result = MakeUpTimeValidator.IsMakeUpTotalTimeConvergingWithPeriod(startTime, endTime, makeUpTimeList);

        Assert.False(result);
    }
}
