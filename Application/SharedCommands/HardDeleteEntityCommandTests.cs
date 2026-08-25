using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.SharedCommands;

[UnitTest]
public class HardDeleteEntityCommandTests
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    private readonly IClaimsProvider _mockClaimsProvider;

    public HardDeleteEntityCommandTests()
    {
        _mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);
    }

    [Fact]
    public async Task DeleteExistingEntityTwice_ShouldDeleteEntityFromDbSetAndDoNotThrowAtSecondTime()
    {
        var (context, connection) = await TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTestsAsync(TENANT_ID);

        await using (context)
        await using (connection)
        {
            var deleteEntityCommand = new HardDeleteEntityCommand(context, _mockClaimsProvider);

            var taskEntry = await context.AddEntityAndSaveAsync(new TaskEntry
            {
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID
            });

            var wasDeleted = await deleteEntityCommand.ExecuteAsync<TaskEntry>(taskEntry.Id);

            var deletedTaskEntry = await context
                .TaskEntries
                .SingleOrDefaultAsync(x => x.Id != taskEntry.Id);

            Assert.True(wasDeleted);
            Assert.Null(deletedTaskEntry);

            var wasDeletedAgain = true;

            // try to delete again
            Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await deleteEntityCommand.ExecuteAsync<TaskEntry>(taskEntry.Id)));
            Assert.False(wasDeletedAgain);
        }
    }

    [Fact]
    public async Task DeleteNonExistingEntity_ShouldNotThrowException()
    {
        var (context, connection) = await TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTestsAsync(TENANT_ID);

        await using (context)
        await using (connection)
        {
            var deleteEntityCommand = new HardDeleteEntityCommand(context, _mockClaimsProvider);

            var wasNonExistedDeleted = true;

            const long NON_EXISTING_ID = -1;

            // try to delete a non-existing entry
            Assert.Null(await Record.ExceptionAsync(async () => wasNonExistedDeleted = await deleteEntityCommand.ExecuteAsync<TaskEntry>(NON_EXISTING_ID)));
            Assert.False(wasNonExistedDeleted);
        }
    }

    [Fact]
    public async Task DeleteAnotherEmployeesEntity_ShouldNotDeleteAnotherEmployeesEntityFromDb()
    {
        var (context, connection) = await TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTestsAsync(TENANT_ID);

        await using (context)
        await using (connection)
        {
            var taskEntry = await context.AddEntityAndSaveAsync(new TaskEntry
            {
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID
            });

            var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(2, TENANT_ID);

            var command = new HardDeleteEntityCommand(context, mockClaimsProvider);

            var wasDeleted = await command.ExecuteAsync<TaskEntry>(taskEntry.Id);

            var taskEntryFromDb = await context
                .TaskEntries
                .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

            Assert.False(wasDeleted);
            Assert.NotNull(taskEntryFromDb);
        }
    }

    [Fact]
    public async Task DeleteAnotherTenantsEntity_ShouldNotDeleteAnotherTenantsEntityFromDb()
    {
        var (context, connection) = await TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTestsAsync(TENANT_ID);

        await using (context)
        await using (connection)
        {

            // To check the tenant isolation, you must specify a TenantId other than 777,
            // since in the implementation of TenantAppDbContextExtensionsTestsRelated,
            // the QueryableWithinTenant method returns TenantId = 777
            var taskEntry = await context.AddEntityAndSaveAsync(new TaskEntry
            {
                EmployeeId = EMPLOYEE_ID,
                TenantId = 2
            });

            var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

            var command = new HardDeleteEntityCommand(context, mockClaimsProvider);

            var wasDeleted = await command.ExecuteAsync<TaskEntry>(taskEntry.Id);

            var taskEntryFromDb = await context
                .TaskEntries
                .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

            Assert.False(wasDeleted);
            Assert.NotNull(taskEntryFromDb);
        }
    }

    [Fact]
    public async Task DeleteEntryThatWasPreviouslyDeletedUsingTheSoftMethod_ShouldDeleteEntityFromDb()
    {
        var (context, connection) = await TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTestsAsync(TENANT_ID);

        await using (context)
        await using (connection)
        {
            var taskEntry = await context.AddEntityAndSaveAsync(new TaskEntry
            {
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                DeletedAtUtc = DateTime.UtcNow
            });

            var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

            var command = new HardDeleteEntityCommand(context, mockClaimsProvider);

            var wasDeleted = await command.ExecuteAsync<TaskEntry>(taskEntry.Id);

            var deletedTaskEntry = await context
                .TaskEntries
                .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

            Assert.True(wasDeleted);
            Assert.Null(deletedTaskEntry);
        }
    }
}
