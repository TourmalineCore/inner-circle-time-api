using Core;
using Xunit;

namespace Application.Validators;

[UnitTest]
public class MakeUpTimeValidatorTests
{
    public static IEnumerable<object[]> MakeUpTotalTimeDoesMatchTestData()
    {
        return new List<object[]>
        {
            // With one make-up time entry
            new object[] {
                new List<MakeUpTimeEntryDto>
                {
                    new MakeUpTimeEntryDto
                    {
                        StartTime = new DateTime(2025, 11, 24, 17, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 18, 30, 0)
                    },
                }
            },
            // With two make-up time entries
            new object[] {
                new List<MakeUpTimeEntryDto>
                {
                    new MakeUpTimeEntryDto
                    {
                        StartTime = new DateTime(2025, 11, 24, 17, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 17, 30, 0)
                    },
                    new MakeUpTimeEntryDto
                    {
                        StartTime = new DateTime(2025, 11, 25, 17, 0, 0),
                        EndTime = new DateTime(2025, 11, 25, 18, 0, 0)
                    }
                }
            }
        };
    }

    [Theory]
    [MemberData(nameof(MakeUpTotalTimeDoesMatchTestData))]
    public async Task MakeUpTimeValidator_ShouldReturnTrueIfMakeUpTotalTimeMatchesWithRelatedEntryPeriod(List<MakeUpTimeEntryDto> makeUpTimeList)
    {
        var startTime = new DateTime(2025, 11, 24, 10, 0, 0);
        var endTime = new DateTime(2025, 11, 24, 11, 30, 0);

        var result = MakeUpTimeValidator.IsMakeUpTotalTimeConvergingWithPeriod(startTime, endTime, makeUpTimeList);

        Assert.True(result);
    }

    public static IEnumerable<object[]> MakeUpTotalTimeDoesNotMatchTestData()
    {
        return new List<object[]>
        {
            // With empty makeUpTimeList
            new object[]
            {
                new List<MakeUpTimeEntryDto>
                {

                }
            },
            // With makeUpTimeList greater than related entry period
            new object[] {
                new List<MakeUpTimeEntryDto>
                {
                    new MakeUpTimeEntryDto
                    {
                        StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 16, 30, 0)
                    }
                }
            },
            // With makeUpTimeList less than related entry period
            new object[] {
                new List<MakeUpTimeEntryDto>
                {
                    new MakeUpTimeEntryDto
                    {
                        StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 13, 30, 0)
                    }
                }
            }
        };
    }

    [Theory]
    [MemberData(nameof(MakeUpTotalTimeDoesNotMatchTestData))]
    public async Task MakeUpTimeValidator_ShouldReturnFalseIfMakeUpTotalTimeDoesNotMatchWithRelatedEntryPeriod(List<MakeUpTimeEntryDto> makeUpTimeList)
    {
        var startTime = new DateTime(2025, 11, 24, 10, 0, 0);
        var endTime = new DateTime(2025, 11, 24, 11, 30, 0);

        var result = MakeUpTimeValidator.IsMakeUpTotalTimeConvergingWithPeriod(startTime, endTime, makeUpTimeList);

        Assert.False(result);
    }
}
