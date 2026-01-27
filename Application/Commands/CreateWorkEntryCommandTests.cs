using Application.TestsConfig;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

[IntegrationTest]
public class CreateWorkEntryCommandTests : IntegrationTestBase
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

        DbUpdateException ex = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams)
        );

        Assert.Contains("ck_work_entries_type_not_zero", ex.InnerException!.Message);
    }
}
