using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

public partial class WorkEntryCommandTestsBase
{
    [Fact]
    public async Task CreateWorkEntryAsync_ShouldThrowErrorIfTypeIsUnspecified()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var createWorkEntryCommand = new CreateWorkEntryCommand(context, mockClaimsProvider);

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            ProjectId = 1,
            TaskId = "#2231",
            Description = "Task description",
            Type = EventType.Unspecified
        };

        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams)
        );

        Assert.Contains("ck_work_entries_type_not_zero", exception.InnerException!.Message);
    }

    [Fact]
    public async Task CreateWorkEntryAsync_ShouldThrowInvalidTimeRangeExceptionIfStartTimeIsGreaterEndTime()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var createWorkEntryCommand = new CreateWorkEntryCommand(context, mockClaimsProvider);

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
            Type = EventType.Task
        };

        var exception = await Assert.ThrowsAsync<InvalidTimeRangeException>(
            async () => await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams)
        );

        Assert.Contains("ck_work_entries_end_time_is_greater_than_start_time", exception.InnerException!.InnerException!.Message);
        Assert.Equal("End time must be greater than start time", exception.Message);
    }

    [Fact]
    public async Task CreateWorkEntryAsync_ShouldThrowConflictingTimeRangeExceptionIfTimeConflictsWithAnotherTask()
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
            ProjectId = 1,
            Description = "Task description",
            Type = EventType.Task
        });

        var createWorkEntryCommand = new CreateWorkEntryCommand(context, mockClaimsProvider);

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 11, 0, 0),
            TaskId = "#2232",
            ProjectId = 1,
            Description = "Task description",
            Type = EventType.Task
        };

        var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
            async () => await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams)
        );

        Assert.Contains("ck_work_entries_no_time_overlap", exception.InnerException!.InnerException!.Message);
        Assert.Equal("Another task is scheduled for this time", exception.Message);
    }
}
