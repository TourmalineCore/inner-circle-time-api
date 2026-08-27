using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.SharedCommands;

[IntegrationTest]
public class HardDeleteEntityCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task DeleteExistingEntityTwice_ShouldDeleteEntityFromDbSetAndDoNotThrowAtSecondTime()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var hardDeleteEntityCommand = new HardDeleteEntityCommand(context, mockClaimsProvider);

        var taskEntry = await context.AddEntityAndSaveAsync(new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
        });

        var wasDeleted = await hardDeleteEntityCommand.ExecuteAsync<TaskEntry>(taskEntry.Id);

        var deletedTaskEntry = await context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id != taskEntry.Id);

        Assert.True(wasDeleted);
        Assert.Null(deletedTaskEntry);

        var wasDeletedAgain = true;

        // try to delete again
        Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await hardDeleteEntityCommand.ExecuteAsync<TaskEntry>(taskEntry.Id)));
        Assert.False(wasDeletedAgain);
    }

    [Fact]
    public async Task DeleteNonExistingEntity_ShouldNotThrowException()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var hardDeleteEntityCommand = new HardDeleteEntityCommand(context, mockClaimsProvider);

        var wasNonExistedDeleted = true;

        const long NON_EXISTING_ID = -1;

        // try to delete a non-existing entry
        Assert.Null(await Record.ExceptionAsync(async () => wasNonExistedDeleted = await hardDeleteEntityCommand.ExecuteAsync<TaskEntry>(NON_EXISTING_ID)));
        Assert.False(wasNonExistedDeleted);
    }

    [Fact]
    public async Task DeleteAnotherEmployeesEntity_ShouldNotDeleteAnotherEmployeesEntityFromDb()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var taskEntry = await context.AddEntityAndSaveAsync(new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 12, 0, 0),
        });

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(2, TENANT_ID);

        var hardDeleteEntityCommand = new HardDeleteEntityCommand(context, mockClaimsProvider);

        var wasDeleted = await hardDeleteEntityCommand.ExecuteAsync<TaskEntry>(taskEntry.Id);

        var taskEntryFromDb = await context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.False(wasDeleted);
        Assert.NotNull(taskEntryFromDb);
    }

    [Fact]
    public async Task DeleteAnotherTenantsEntity_ShouldNotDeleteAnotherTenantsEntityFromDb()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        // To check the tenant isolation, you must specify a TenantId other than 777,
        // since in the implementation of TenantAppDbContextExtensionsTestsRelated,
        // the QueryableWithinTenant method returns TenantId = 777
        var taskEntry = await context.AddEntityAndSaveAsync(new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = 2
        });

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var hardDeleteEntityCommand = new HardDeleteEntityCommand(context, mockClaimsProvider);

        var wasDeleted = await hardDeleteEntityCommand.ExecuteAsync<TaskEntry>(taskEntry.Id);

        var taskEntryFromDb = await context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.False(wasDeleted);
        Assert.NotNull(taskEntryFromDb);
    }

    [Fact]
    public async Task DeleteEntryThatWasPreviouslyDeletedUsingTheSoftMethod_ShouldDeleteEntityFromDb()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var taskEntry = await context.AddEntityAndSaveAsync(new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            DeletedAtUtc = DateTime.UtcNow
        });

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var hardDeleteEntityCommand = new HardDeleteEntityCommand(context, mockClaimsProvider);

        var wasDeleted = await hardDeleteEntityCommand.ExecuteAsync<TaskEntry>(taskEntry.Id);

        var deletedTaskEntry = await context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.True(wasDeleted);
        Assert.Null(deletedTaskEntry);
    }
}
