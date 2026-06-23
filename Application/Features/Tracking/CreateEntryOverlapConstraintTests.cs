using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.CreateUnwellEntry;
using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.Tracking;

[IntegrationTest]
public class CreateEntryOverlapConstraintTests : IntegrationTestBase
{
    [Theory]
    [MemberData(nameof(OverlapTestData))]
    public async Task CreateEntryAsync_ShouldRespectOverlapConstraint(
        TrackedEntryBase entry,
        Func<TenantAppDbContext, IClaimsProvider, Task> command,
        bool canOverlap
    )
    {
        var context = CreateTenantDbContext();
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        await SaveEntityAsync(context, entry);

        if (!canOverlap)
        {
            var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
                async () => await command(context, mockClaimsProvider)
            );

            Assert.Equal("Another task is scheduled for this time", exception.Message);
        }
        else
        {
            var exception = await Record.ExceptionAsync(
                async () => await command(context, mockClaimsProvider)
            );

            Assert.Null(exception);
        }
    }

    public static IEnumerable<object[]> OverlapTestData()
    {
        var entryTypesWithoutUnspecified = Enum.GetValues<EntryType>()
            .Where(x => x != EntryType.Unspecified)
            .ToArray();

        foreach (EntryType existingEntryType in entryTypesWithoutUnspecified)
            foreach (EntryType createdEntryType in entryTypesWithoutUnspecified)
            {
                yield return new object[]
                {
                    CreateExistingEntry(existingEntryType),
                    CreateEntryCommand(createdEntryType),
                    IsOverlapAllowed(existingEntryType, createdEntryType)
                };
            }
    }

    public static TrackedEntryBase CreateExistingEntry(EntryType entryType)
    {
        var startTime = new DateTime(2026, 11, 24, 9, 0, 0);
        var endTime = new DateTime(2026, 11, 24, 11, 0, 0);

        return entryType switch
        {
            EntryType.Task => new TaskEntry
            {
                EmployeeId = EMPLOYEE_ID,
                Title = "Existing Task",
                StartTime = startTime,
                EndTime = endTime,
                TaskId = "#2231",
                ProjectId = 1,
                Description = "Task description",
            },
            EntryType.Unwell => new UnwellEntry
            {
                EmployeeId = EMPLOYEE_ID,
                StartTime = startTime,
                EndTime = endTime,
            },
            EntryType.AwayWithMakeUpTime => new AwayWithMakeUpTimeEntry
            {
                EmployeeId = EMPLOYEE_ID,
                StartTime = startTime,
                EndTime = endTime,
                Description = "Description",
                MakeUpTimeList = []
            },
            // MakeUpTimeEntry can't be saved without a related linked record.
            // Therefore, to create a make up time entry через away entry.
            EntryType.MakeUpTime => new AwayWithMakeUpTimeEntry
            {
                EmployeeId = EMPLOYEE_ID,
                StartTime = new DateTime(1999, 11, 24, 17, 0, 0),
                EndTime = new DateTime(1999, 11, 24, 18, 0, 0),
                Description = "Description",
                MakeUpTimeList =
                [
                    new MakeUpTimeEntry
                    {
                        TenantId = TENANT_ID,
                        EmployeeId = EMPLOYEE_ID,
                        StartTime = startTime,
                        EndTime = endTime
                    }
                ]
            },
            _ => throw new Exception($"The test configuration is not configured to work with {entryType}."),
        };
    }

    public static Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand(EntryType entryType)
    {
        var startTime = new DateTime(2026, 11, 24, 9, 0, 0);
        var endTime = new DateTime(2026, 11, 24, 11, 0, 0);

        return entryType switch
        {
            EntryType.Task =>
             (context, claimsProvider) => new CreateTaskEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateTaskEntryRequest
                {
                    Title = "New Task",
                    StartTime = startTime,
                    EndTime = endTime,
                    TaskId = "#222",
                    ProjectId = 1,
                    Description = "Description"
                }),
            EntryType.Unwell =>
             (context, claimsProvider) => new CreateUnwellEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateUnwellEntryRequest
                {
                    StartTime = startTime,
                    EndTime = endTime,
                }),
            EntryType.AwayWithMakeUpTime =>
             (context, claimsProvider) => new CreateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateAwayWithMakeUpTimeEntryRequest
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    Description = "Description",
                    MakeUpTimeList = []
                }),
            // MakeUpTimeEntry can't be saved without a related linked record.
            // It does't have its own command to create, it is always created as part of another entry.
            // Therefore, to create a make up time entry, we use the away command.
            EntryType.MakeUpTime =>
             (context, claimsProvider) => new CreateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                .ExecuteAsync(new CreateAwayWithMakeUpTimeEntryRequest
                {
                    StartTime = new DateTime(1999, 11, 24, 15, 0, 0),
                    EndTime = new DateTime(1999, 11, 24, 16, 0, 0),
                    Description = "Description",
                    MakeUpTimeList = [
                    new MakeUpTimeEntryDto
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                        }
                    ]
                }),
            _ => throw new Exception($"The test configuration is not configured to work with {entryType}."),
        };
    }

    private static bool IsOverlapAllowed(EntryType existingEntryType, EntryType createdEntryType)
    {
        if (existingEntryType == EntryType.Task && createdEntryType == EntryType.MakeUpTime ||
            existingEntryType == EntryType.MakeUpTime && createdEntryType == EntryType.Task
        )
        {
            return true;
        }

        return false;
    }
}
