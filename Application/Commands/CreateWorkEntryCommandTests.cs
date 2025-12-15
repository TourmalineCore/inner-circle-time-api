using Application.TestsConfig;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

[IntegrationTest]
public class CreateWorkEntryCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateWorkEntryAsync_ShouldSaveNewWorkEntryToDb()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = GetMockClaimsProvider();

        var createWorkEntryCommand = new CreateWorkEntryCommand(context, mockClaimsProvider);

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            Description = "Task description",
            Type = EventType.Task
        };

        var newWorkEntryId = await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams);

        var newWorkEntry = await FindEntityAsync<WorkEntry>(context, newWorkEntryId);

        Assert.NotNull(newWorkEntry);
        Assert.Equal(createWorkEntryCommandParams.Title, newWorkEntry.Title);
        Assert.Equal(EMPLOYEE_ID, newWorkEntry.EmployeeId);
        Assert.Equal(TENANT_ID, newWorkEntry.TenantId);
        Assert.Equal(createWorkEntryCommandParams.TaskId, newWorkEntry.TaskId);
        Assert.Equal(createWorkEntryCommandParams.StartTime, newWorkEntry.StartTime);
        Assert.Equal(createWorkEntryCommandParams.EndTime, newWorkEntry.EndTime);
        Assert.Equal(createWorkEntryCommandParams.Type, newWorkEntry.Type);
        Assert.Equal(createWorkEntryCommandParams.Description, newWorkEntry.Description);
        Assert.Equal(createWorkEntryCommandParams.EndTime - createWorkEntryCommandParams.StartTime, newWorkEntry.Duration);
    }

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
            TaskId = "#2231",
            Description = "Task description",
            Type = EventType.Unspecified
        };

        DbUpdateException ex = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams)
        );

        Assert.Contains("ck_work_entries_type_not_zero", ex.InnerException?.Message);
    }
}
