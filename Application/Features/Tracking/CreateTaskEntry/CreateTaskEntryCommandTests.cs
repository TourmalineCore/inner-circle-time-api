using Core.Entities;
using Xunit;

namespace Application.Features.Tracking.CreateTaskEntry;
// Before that there were 2 separate classes and they were running concurrently and often lead to a deadlock and thus tests were flaky
// There is an issue to investigate the root cause why they fail https://github.com/TourmalineCore/inner-circle-time-api/issues/26
// Current solution assigns tests to a collection to disable parallelization, 
// preventing conflicts when accessing shared resources
//https://xunit.net/docs/running-tests-in-parallel
[Collection("EntryCommandTests")]
public class CreateTaskEntryCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateTaskEntryAsync_ShouldThrowInvalidTimeRangeExceptionIfStartTimeIsGreaterEndTime()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var createTaskEntryCommand = new CreateTaskEntryCommand(context, mockClaimsProvider);

        var сreateTaskEntryRequest = new CreateTaskEntryRequest
        {
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            ProjectId = 1,
            Description = "Task description",
        };

        var exception = await Assert.ThrowsAsync<InvalidTimeRangeException>(
            async () => await createTaskEntryCommand.ExecuteAsync(сreateTaskEntryRequest)
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

        var сreateTaskEntryRequest = new CreateTaskEntryRequest
        {
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 11, 0, 0),
            TaskId = "#2232",
            ProjectId = 1,
            Description = "Task description",
        };

        var exception = await Assert.ThrowsAsync<ConflictingTimeRangeException>(
            async () => await createTaskEntryCommand.ExecuteAsync(сreateTaskEntryRequest)
        );

        Assert.Contains("ck_entries_task_unwell_no_time_overlap", exception.InnerException!.InnerException!.Message);
        Assert.Equal("Another task is scheduled for this time", exception.Message);
    }
}
