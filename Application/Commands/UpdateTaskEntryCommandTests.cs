using Core.Entities;
using Xunit;

namespace Application.Commands;

public partial class EntryCommandTestsBase
{
    [Fact]
    public async Task UpdateTaskEntryAsync_ShouldThrowInvalidTimeRangeExceptionIfStartTimeIsGreaterEndTime()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var updateTaskEntryCommand = new UpdateTaskEntryCommand(context, mockClaimsProvider);

        var taskEntry = await SaveEntityAsync(context, new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
        });

        var updateTaskEntryCommandParams = new UpdateTaskEntryCommandParams
        {
            Id = taskEntry.Id,
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 25, 12, 0, 0),
            EndTime = new DateTime(2025, 11, 25, 11, 0, 0),
            TaskId = "#22",
            ProjectId = 2,
            Description = "Task description",
        };

        var exception = await Assert.ThrowsAsync<InvalidTimeRangeException>(
            async () => await updateTaskEntryCommand.ExecuteAsync(updateTaskEntryCommandParams)
        );

        Assert.Contains("ck_entries_end_time_is_greater_than_start_time", exception.InnerException!.Message);
        Assert.Equal("End time must be greater than start time", exception.Message);
    }

    [Fact]
    public async Task UpdateTaskEntryAsync_ShouldThrowConflictingTimeRangeExceptionIfTimeConflictsWithAnotherTask()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var taskEntry = await SaveEntityAsync(context, new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
        });

        var taskEntry2 = await SaveEntityAsync(context, new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
        });

        var updateTaskEntryCommand = new UpdateTaskEntryCommand(context, mockClaimsProvider);

        var updateTaskEntryCommandParams = new UpdateTaskEntryCommandParams
        {
            Id = taskEntry2.Id,
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 11, 0, 0),
            TaskId = "#2232",
            ProjectId = 1,
            Description = "Task description",
        };

        var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
            async () => await updateTaskEntryCommand.ExecuteAsync(updateTaskEntryCommandParams)
        );

        Assert.Contains("ck_entries_type12_no_time_overlap", exception.InnerException!.Message);
        Assert.Equal("Another task is scheduled for this time", exception.Message);
    }
}
