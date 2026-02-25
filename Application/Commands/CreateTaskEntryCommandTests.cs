using Core.Entities;
using Xunit;

namespace Application.Commands;

public partial class EntryCommandTestsBase
{
    [Fact]
    public async Task CreateTaskEntryAsync_ShouldThrowInvalidTimeRangeExceptionIfStartTimeIsGreaterEndTime()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var createTaskEntryCommand = new CreateTaskEntryCommand(context, mockClaimsProvider);

        var createTaskEntryCommandParams = new CreateTaskEntryCommandParams
        {
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
        };

        var exception = await Assert.ThrowsAsync<InvalidTimeRangeException>(
            async () => await createTaskEntryCommand.ExecuteAsync(createTaskEntryCommandParams)
        );

        Assert.Contains("ck_entries_end_time_is_greater_than_start_time", exception.InnerException!.InnerException!.Message);
        Assert.Equal("End time must be greater than start time", exception.Message);
    }

    [Fact]
    public async Task CreateTaskEntryAsync_ShouldThrowConflictingTimeRangeExceptionIfTimeConflictsWithAnotherTask()
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

        var createTaskEntryCommand = new CreateTaskEntryCommand(context, mockClaimsProvider);

        var createTaskEntryCommandParams = new CreateTaskEntryCommandParams
        {
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 11, 0, 0),
            TaskId = "#2232",
            ProjectId = 1,
            Description = "Task description",
        };

        var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
            async () => await createTaskEntryCommand.ExecuteAsync(createTaskEntryCommandParams)
        );

        Assert.Contains("ck_entries_type12_no_time_overlap", exception.InnerException!.InnerException!.Message);
        Assert.Equal("Another task is scheduled for this time", exception.Message);
    }
}
