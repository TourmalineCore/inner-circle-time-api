using Application.TestsConfig;
using Core.Entities;
using Xunit;

namespace Application.Commands;

[IntegrationTest]
public class UpdateWorkEntryCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task UpdateWorkEntryAsync_ShouldUpdateWorkEntryDataInDb()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var workEntry = await SaveEntityAsync(context, new WorkEntry
        {
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            Description = "Task description",
            Type = EventType.Task
        });

        var updateWorkEntryCommandParams = new UpdateWorkEntryCommandParams
        {
            Id = workEntry.Id,
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 25, 8, 0, 0),
            EndTime = new DateTime(2025, 11, 25, 11, 0, 0),
            TaskId = "#22",
            Description = "Task description",
            Type = EventType.Task
        };

        var updateWorkEntryCommand = new UpdateWorkEntryCommand(context, mockClaimsProvider);

        await updateWorkEntryCommand.ExecuteAsync(updateWorkEntryCommandParams);

        var updatedWorkEntry = await FindEntityAsync<WorkEntry>(context, workEntry.Id);

        Assert.NotNull(updatedWorkEntry);
        Assert.Equal(updateWorkEntryCommandParams.Title, updatedWorkEntry.Title);
        Assert.Equal(updateWorkEntryCommandParams.TaskId, updatedWorkEntry.TaskId);
        Assert.Equal(updateWorkEntryCommandParams.StartTime, updatedWorkEntry.StartTime);
        Assert.Equal(updateWorkEntryCommandParams.EndTime, updatedWorkEntry.EndTime);
        Assert.Equal(updateWorkEntryCommandParams.Type, updatedWorkEntry.Type);
        Assert.Equal(updateWorkEntryCommandParams.Description, updatedWorkEntry.Description);
        Assert.Equal(updateWorkEntryCommandParams.EndTime - updateWorkEntryCommandParams.StartTime, updatedWorkEntry.Duration);
    }

    [Fact]
    public async Task UpdateWorkEntryAsync_ShouldThrowInvalidTimeRangeExceptionIfStartTimeIsGreaterEndTime()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var updateWorkEntryCommand = new UpdateWorkEntryCommand(context, mockClaimsProvider);

        var workEntry = await SaveEntityAsync(context, new WorkEntry
        {
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            Description = "Task description",
            Type = EventType.Task
        });

        var updateWorkEntryCommandParams = new UpdateWorkEntryCommandParams
        {
            Id = workEntry.Id,
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 25, 12, 0, 0),
            EndTime = new DateTime(2025, 11, 25, 11, 0, 0),
            TaskId = "#22",
            Description = "Task description",
            Type = EventType.Task
        };

        var exception = await Assert.ThrowsAsync<InvalidTimeRangeException>(
            async () => await updateWorkEntryCommand.ExecuteAsync(updateWorkEntryCommandParams)
        );

        Assert.Contains("ck_work_entries_end_time_is_greater_than_start_time", exception.InnerException?.Message);
        Assert.Equal("End time must be greater than start time", exception.Message);
    }

    [Fact]
    public async Task UpdateWorkEntryAsync_ShouldThrowConflictingTimeRangeExceptionIfTimeConflictsWithAnotherTask()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var workEntry1 = await SaveEntityAsync(context, new WorkEntry
        {
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            Description = "Task description",
            Type = EventType.Task
        });

        var workEntry2 = await SaveEntityAsync(context, new WorkEntry
        {
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
            TaskId = "#2231",
            Description = "Task description",
            Type = EventType.Task
        });

        var updateWorkEntryCommand = new UpdateWorkEntryCommand(context, mockClaimsProvider);

        var updateEntryCommandParams = new UpdateWorkEntryCommandParams
        {
            Id = workEntry1.Id,
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
            TaskId = "#2232",
            Description = "Task description",
            Type = EventType.Task
        };

        var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
            async () => await updateWorkEntryCommand.ExecuteAsync(updateEntryCommandParams)
        );

        Assert.Contains("ck_work_entries_no_time_overlap", exception.InnerException?.Message);
        Assert.Equal("The time conflicts with the time of another task", exception.Message);
    }
}
