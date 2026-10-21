using Core;
using Core.Features.Tracking.Entities;
using Xunit;

namespace Application.Features.Tracking.Formatters;

[UnitTest]
public class OverlapErrorMessageFormatterTest
{
    [Fact]
    public void GetErrorMessage_WhenNonAllDayEntryOverlappedWithNonAllDayEntry_ShouldReturnCorrectMessage()
    {
        var result = OverlapErrorMessageFormatter.GetErrorMessage(
            new TaskEntry
            {
                StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 12, 0, 0),
            },
            new UnwellEntry
            {
                StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 10, 0, 0),
            }
        );

        Assert.Equal("Unwell is already tracked from 09:00-10:00. Adjust the time or edit the existing entry.", result);
    }

    [Fact]
    public void GetErrorMessage_WhenNonAllDayEntryOverlappedWithAllDayEntry_ShouldReturnCorrectMessage()
    {
        var result = OverlapErrorMessageFormatter.GetErrorMessage(
            new TaskEntry
            {
                StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 12, 0, 0),
            },
            new SickLeaveEntry
            {
                StartTime = new DateTime(2026, 10, 20, 0, 0, 0),
                EndTime = new DateTime(2026, 10, 24, 0, 0, 0),
            }
        );

        Assert.Equal("Task cannot be tracked on the day of sick leave. Adjust the time or edit the existing entry.", result);
    }

    [Fact]
    public void GetErrorMessage_WhenAllDayEntryOverlappedWithAllDayEntry_ShouldReturnCorrectMessage()
    {
        var result = OverlapErrorMessageFormatter.GetErrorMessage(
            new VacationEntry
            {
                StartTime = new DateTime(2026, 10, 21, 0, 0, 0),
                EndTime = new DateTime(2026, 10, 22, 0, 0, 0),
            },
            new SickLeaveEntry
            {
                StartTime = new DateTime(2026, 10, 20, 0, 0, 0),
                EndTime = new DateTime(2026, 10, 24, 0, 0, 0),
            }
        );

        Assert.Equal("Vacation cannot be tracked on the day of sick leave. Adjust the time or edit the existing entry.", result);
    }

    [Fact]
    public void GetErrorMessage_WhenAllDayEntryOverlappedWithNonAllDayEntry_ShouldReturnCorrectMessage()
    {
        var result = OverlapErrorMessageFormatter.GetErrorMessage(
            new SickLeaveEntry
            {
                StartTime = new DateTime(2026, 10, 21, 0, 0, 0),
                EndTime = new DateTime(2026, 10, 22, 0, 0, 0),
            },
            new MakeUpTimeEntry
            {
                StartTime = new DateTime(2026, 10, 21, 8, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 9, 0, 0),
            }
        );

        Assert.Equal("Make-up time is already tracked from 08:00-09:00. Adjust the time or edit the existing entry.", result);
    }

    [Fact]
    public void GetErrorMessage_ShouldNotThrowMissingMappingException_ForAnyEntryType()
    {
        var entryTypesWithoutUnspecified = Enum.GetValues<EntryType>()
            .Where(x => x != EntryType.Unspecified)
            .ToList();

        foreach (EntryType entryType in entryTypesWithoutUnspecified)
        {
            var entry = new TrackedEntryBase
            {
                Type = entryType,
                StartTime = new DateTime(2026, 10, 21, 9, 0, 0),
                EndTime = new DateTime(2026, 10, 21, 10, 0, 0),
            };

            var result = OverlapErrorMessageFormatter.GetErrorMessage(entry, entry);

            Assert.NotEmpty(result);
        }
    }
}
