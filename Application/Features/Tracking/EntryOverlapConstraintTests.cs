using Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.CreateTaskEntry;
using Application.Features.Tracking.CreateUnwellEntry;
using Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;
using Application.Features.Tracking.UpdateTaskEntry;
using Application.Features.Tracking.UpdateUnwellEntry;
using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.Tracking;

[IntegrationTest]
public class EntryOverlapConstraintTests : IntegrationTestBase
{
    [Theory]
    [MemberData(nameof(CreateOverlapTestData))]
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

    public static IEnumerable<object[]> CreateOverlapTestData()
    {
        var entryTypesWithoutUnspecified = Enum.GetValues<EntryType>()
            .Where(x => x != EntryType.Unspecified)
            .ToArray();

        var startTime = new DateTime(2026, 11, 24, 9, 0, 0);
        var endTime = new DateTime(2026, 11, 24, 11, 0, 0);

        foreach (EntryType existingEntryType in entryTypesWithoutUnspecified)
            foreach (EntryType createdEntryType in entryTypesWithoutUnspecified)
            {
                yield return new object[]
                {
                    CreateExistingEntry(existingEntryType, startTime, endTime),
                    CreateEntryCommand(createdEntryType, startTime, endTime),
                    IsOverlapAllowed(existingEntryType, createdEntryType)
                };
            }
    }

    [Theory]
    [MemberData(nameof(UpdateOverlapTestData))]
    public async Task UpdateEntryAsync_ShouldRespectOverlapConstraint(
        TrackedEntryBase entry,
        TrackedEntryBase entryToUpdate,
        Func<TenantAppDbContext, IClaimsProvider, long, Task> command,
        bool canOverlap
    )
    {
        var context = CreateTenantDbContext();
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        await SaveEntityAsync(context, entry);

        var entryIdToUpdate = (await SaveEntityAsync(context, entryToUpdate)).Id;

        if (!canOverlap)
        {
            var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
                async () => await command(context, mockClaimsProvider, entryIdToUpdate)
            );

            Assert.Equal("Another task is scheduled for this time", exception.Message);
        }
        else
        {
            var exception = await Record.ExceptionAsync(
                async () => await command(context, mockClaimsProvider, entryIdToUpdate)
            );

            Assert.Null(exception);
        }
    }

    public static IEnumerable<object[]> UpdateOverlapTestData()
    {
        var entryTypesWithoutUnspecified = Enum.GetValues<EntryType>()
            .Where(x => x != EntryType.Unspecified)
            .ToArray();

        var startTime = new DateTime(2026, 10, 24, 9, 0, 0);
        var endTime = new DateTime(2026, 10, 24, 11, 0, 0);

        var startTimeForEntryToUpdate = new DateTime(2026, 10, 23, 9, 0, 0);
        var endTimeForEntryToUpdate = new DateTime(2026, 10, 23, 11, 0, 0);

        foreach (EntryType existingEntryType in entryTypesWithoutUnspecified)
            foreach (EntryType createdEntryType in entryTypesWithoutUnspecified)
            {
                yield return new object[]
                {
                    CreateExistingEntry(existingEntryType, startTime, endTime),
                    CreateExistingEntry(createdEntryType, startTimeForEntryToUpdate, endTimeForEntryToUpdate),
                    UpdateEntryCommand(createdEntryType, startTime, endTime),
                    IsOverlapAllowed(existingEntryType, createdEntryType)
                };
            }
    }

    public static TrackedEntryBase CreateExistingEntry(
        EntryType entryType,
        DateTime startTime,
        DateTime endTime
    )
    {
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
            // Therefore, to create a make up time entry via away entry.
            EntryType.MakeUpTime => new AwayWithMakeUpTimeEntry
            {
                EmployeeId = EMPLOYEE_ID,
                StartTime = startTime.AddYears(-10),
                EndTime = endTime.AddYears(-10),
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
            _ => throw new Exception($"The test is not configured to work with {entryType}."),
        };
    }

    public static Func<TenantAppDbContext, IClaimsProvider, Task> CreateEntryCommand(
        EntryType entryType,
        DateTime startTime,
        DateTime endTime
    )
    {
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
            _ => throw new Exception($"The test is not configured to work with {entryType}."),
        };
    }

    public static Func<TenantAppDbContext, IClaimsProvider, long, Task> UpdateEntryCommand(
        EntryType entryType,
        DateTime startTime,
        DateTime endTime
    )
    {
        return entryType switch
        {
            EntryType.Task =>
             (context, claimsProvider, entryId) => new UpdateTaskEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateTaskEntryRequest
                {
                    Id = entryId,
                    Title = "New Task",
                    StartTime = startTime,
                    EndTime = endTime,
                    TaskId = "#222",
                    ProjectId = 1,
                    Description = "Description"
                }),
            EntryType.Unwell =>
             (context, claimsProvider, entryId) => new UpdateUnwellEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateUnwellEntryRequest
                {
                    Id = entryId,
                    StartTime = startTime,
                    EndTime = endTime,
                }),
            EntryType.AwayWithMakeUpTime =>
             (context, claimsProvider, entryId) => new UpdateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateAwayWithMakeUpTimeEntryRequest
                {
                    Id = entryId,
                    StartTime = startTime,
                    EndTime = endTime,
                    Description = "Description",
                    MakeUpTimeList = []
                }),
            // MakeUpTimeEntry can't be saved without a related linked record.
            // It does't have its own command to create, it is always created as part of another entry.
            // Therefore, to create a make up time entry, we use the away command.
            EntryType.MakeUpTime =>
             (context, claimsProvider, entryId) => new UpdateAwayWithMakeUpTimeEntryCommand(context, claimsProvider)
                .ExecuteAsync(new UpdateAwayWithMakeUpTimeEntryRequest
                {
                    Id = entryId,
                    StartTime = new DateTime(1999, 10, 24, 15, 0, 0),
                    EndTime = new DateTime(1999, 10, 24, 16, 0, 0),
                    Description = "Description",
                    MakeUpTimeList = [
                    new MakeUpTimeEntryDto
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                        }
                    ]
                }),
            _ => throw new Exception($"The test is not configured to work with {entryType}."),
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
