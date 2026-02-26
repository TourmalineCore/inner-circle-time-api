using Application.TestsConfig;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.Commands;

[UnitTest]
public class SoftDeleteEntryCommandTests
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    private readonly SoftDeleteEntryCommand _command;
    private readonly TenantAppDbContext _context;

    public SoftDeleteEntryCommandTests()
    {
        _context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);
        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);

        _command = new SoftDeleteEntryCommand(_context, mockClaimsProvider.Object);
    }

    [Fact]
    public async Task SoftDeleteExistingEntryTwice_ShouldDeleteEntryFromDbSetAndDoNotThrowAtSecondTime()
    {
        var taskEntry = await _context.AddEntityAndSaveAsync(new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID
        });

        var wasDeleted = await _command.ExecuteAsync(taskEntry.Id);

        Assert.True(wasDeleted);

        var deletedTaskEntry = await _context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.NotNull(deletedTaskEntry);
        Assert.NotNull(deletedTaskEntry.DeletedAtUtc);

        var wasDeletedAgain = true;

        // try to delete again
        Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await _command.ExecuteAsync(taskEntry.Id)));
        Assert.False(wasDeletedAgain);
    }

    [Fact]
    public async Task SoftDeleteNonExistingEntry_ShouldNotThrowException()
    {
        var wasNonExistedDeleted = true;

        const long NON_EXISTING_ID = -1;

        // try to delete a non-existing entry
        Assert.Null(await Record.ExceptionAsync(async () => wasNonExistedDeleted = await _command.ExecuteAsync(NON_EXISTING_ID)));
        Assert.False(wasNonExistedDeleted);
    }

    [Fact]
    public async Task SoftDeleteAnotherEmployeesEntry_ShouldNotDeleteAnotherEmployeesEntryFromDb()
    {
        var taskEntry = await _context.AddEntityAndSaveAsync(new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID
        });

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(2);

        var command = new HardDeleteEntityCommand(_context, mockClaimsProvider.Object);

        var wasDeleted = await command.ExecuteAsync<TaskEntry>(taskEntry.Id);

        var taskEntryFromDb = await _context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.False(wasDeleted);
        Assert.NotNull(taskEntryFromDb);
    }

    [Fact]
    public async Task SoftDeleteAnotherTenantsEntry_ShouldNotDeleteAnotherTenantsEntryFromDb()
    {
        // To check the tenant isolation, you must specify a TenantId other than 777,
        // since in the implementation of TenantAppDbContextExtensionsTestsRelated,
        // the QueryableWithinTenant method returns TenantId = 777
        var taskEntry = await _context.AddEntityAndSaveAsync(new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = 2
        });

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var command = new HardDeleteEntityCommand(_context, mockClaimsProvider.Object);

        var wasDeleted = await command.ExecuteAsync<TaskEntry>(taskEntry.Id);

        var taskEntryFromDb = await _context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.False(wasDeleted);
        Assert.NotNull(taskEntryFromDb);
    }
}
