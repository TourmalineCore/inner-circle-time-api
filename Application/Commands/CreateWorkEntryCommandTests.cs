using Core.Entities;
using Moq;
using Xunit;

namespace Application.Commands;

public class CreateWorkEntryCommandTests
{
    private const long EMPLOYEE_ID = 1;

    [Fact]
    public async Task CreateWorkEntryAsync_ShouldAddNewWorkEntryToDbSet()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests();

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(cp => cp.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var createWorkEntryCommand = new CreateWorkEntryCommand(context, mockClaimsProvider.Object);

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            Type = EventType.Task
        };

        var newWorkEntryId = await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams);

        var newWorkEntry = await context
            .WorkEntries
            .FindAsync(newWorkEntryId);

        Assert.NotNull(newWorkEntry);
        Assert.Equal(createWorkEntryCommandParams.Title, newWorkEntry.Title);
        Assert.Equal(EMPLOYEE_ID, newWorkEntry.EmployeeId);
        Assert.Equal(createWorkEntryCommandParams.TaskId, newWorkEntry.TaskId);
        Assert.Equal(createWorkEntryCommandParams.StartTime, newWorkEntry.StartTime);
        Assert.Equal(createWorkEntryCommandParams.EndTime, newWorkEntry.EndTime);
        Assert.Equal(createWorkEntryCommandParams.Type, newWorkEntry.Type);

        // Not checked in InMemoryDb
        // Assert.Equal(createWorkEntryCommandParams.EndTime - createWorkEntryCommandParams.StartTime, newWorkEntry.Duration);
    }

    [Fact]
    public async Task CreateWithoutRequiredFieldsAsync_ShouldThrowException()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests();

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider.Setup(cp => cp.EmployeeId)
                          .Returns(EMPLOYEE_ID);

        var createWorkEntryCommand = new CreateWorkEntryCommand(context, mockClaimsProvider.Object);

        var createWorkEntryCommandParams = new CreateWorkEntryCommandParams
        {
            Title = "",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            Type = EventType.Task
        };

        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(
            async () => await createWorkEntryCommand.ExecuteAsync(createWorkEntryCommandParams)
        );

        var exceptionMessage = "Fill in all fields";
        Assert.Equal(exceptionMessage, ex.Message);
    }
}
