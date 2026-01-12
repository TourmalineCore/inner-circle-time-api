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

        InvalidTimeRangeException ex = await Assert.ThrowsAsync<InvalidTimeRangeException>(
            async () => await updateWorkEntryCommand.ExecuteAsync(updateWorkEntryCommandParams)
        );

        Assert.Contains("ck_work_entries_end_time_is_greater_than_start_time", ex.InnerException?.Message);
        Assert.Equal("End time must be greater than start time", ex.Message);
    }
}
