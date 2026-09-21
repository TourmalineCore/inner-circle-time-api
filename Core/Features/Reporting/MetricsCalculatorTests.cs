using Core.Features.Tracking.Entities;
using Xunit;

namespace Core.Features.Reporting;

[UnitTest]
public class MetricsCalculatorTests
{
    private static readonly HashSet<EntryType> _entryTypesCountedInMetrics = new()
    {
        EntryType.Task,
        EntryType.Unwell,
    };

    public static IEnumerable<object[]> TrackedEntriesData()
    {
        return new List<object[]>
        {
            // Empty list
            new object[]
            {
                new List<TrackedEntryBase> { },
                0m
            },

            // Only Task
            new object[]
            {
                new List<TrackedEntryBase>
                {
                    new TaskEntry
                    {
                        StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 9, 30, 0),
                    }
                },
                0.5m
            },

            // Only Unwell
            new object[]
            {
                new List<TrackedEntryBase>
                {
                    new UnwellEntry
                    {
                        StartTime = new DateTime(2025, 11, 24, 10, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 11, 20, 0),
                    }
                },
                1.3333333333333333333333333333m
            },

            // Unwell and Task
            new object[]
            {
                new List<TrackedEntryBase>
                {
                    new UnwellEntry
                    {
                        StartTime = new DateTime(2025, 11, 24, 10, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 11, 20, 0),
                    },
                    new TaskEntry
                    {
                        StartTime = new DateTime(2025, 11, 24, 11, 20, 0),
                        EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
                    }
                },
                2m
            },

            // Unwell and two Task
            new object[]
            {
                new List<TrackedEntryBase>
                {
                    new UnwellEntry
                    {
                        StartTime = new DateTime(2025, 11, 24, 8, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 8, 20, 0),
                    },
                    new TaskEntry
                    {
                        StartTime = new DateTime(2025, 11, 24, 8, 20, 0),
                        EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
                    },
                    new TaskEntry
                    {
                        StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 17, 0, 0),
                    }
                },
                8m
            }
        };
    }

    [Theory]
    [MemberData(nameof(TrackedEntriesData))]
    public void Calculate_ShouldReturnExpectedTrackedHours(
       List<TrackedEntryBase> trackedEntries,
       decimal expectedTrackedHours
    )
    {
        var metrics = MetricsCalculator.Calculate(trackedEntries);

        Assert.Equal(expectedTrackedHours, metrics.TrackedHours);
    }

    [Fact]
    public void Calculate_ShouldIgnoreEntryTypesNotCountedInMetrics()
    {
        var entryTypesNotCountedInMetrics = Enum.GetValues<EntryType>()
            .Where(x => !_entryTypesCountedInMetrics.Contains(x) && x != EntryType.Unspecified);

        var trackedEntries = entryTypesNotCountedInMetrics
            .Select(entryType =>
            {
                var entry = CreateEntry(entryType);
                // In this test, we can allow records to have the same date and time
                // because here we are testing the metrics calculator, not the validation for overlaps.
                entry.StartTime = new DateTime(2025, 11, 24, 9, 0, 0);
                entry.EndTime = new DateTime(2025, 11, 24, 10, 0, 0);
                return entry;
            })
            .ToList();

        var metrics = MetricsCalculator.Calculate(trackedEntries);

        Assert.Equal(0m, metrics.TrackedHours);
    }

    private static TrackedEntryBase CreateEntry(EntryType entryType)
    {
        return entryType switch
        {
            EntryType.AwayWithMakeUpTime => new AwayWithMakeUpTimeEntry(),
            EntryType.MakeUpTime => new MakeUpTimeEntry(),
            EntryType.SickLeave => new SickLeaveEntry(),
            EntryType.Vacation => new VacationEntry(),
            EntryType.Task => new TaskEntry(),
            EntryType.Unwell => new UnwellEntry(),
            _ => throw new Exception($"The test is not configured to work with {entryType}."),
        };
    }
}
