using Xunit;

namespace Application.Commands;

public class CreateWorkEntryCommandTests
{
    [Fact]
    public async Task CreateWorkEntryAsync_ShouldAddNewWorkEntryToDbSet()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests();

        var createWorkEntryCommand = new CreateWorkEntryCommand(context);

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            EmployeeId = 1,
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231"
        };

        var newWorkEntryId = await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams);

        var newWorkEntry = await context.WorkEntries.FindAsync(newWorkEntryId);

        Assert.NotNull(newWorkEntry);
        Assert.Equal(createWorkEntryCommandParams.Title, newWorkEntry.Title);
        Assert.Equal(createWorkEntryCommandParams.TaskId, newWorkEntry.TaskId);
    }
}
