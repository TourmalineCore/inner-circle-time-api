using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.SoftDeleteEntry;

[UnitTest]
public class SoftDeleteEntryCommandTests
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    private readonly SoftDeleteEntryCommand _command;
    private readonly TenantAppDbContext _context;

    private readonly SoftDeleteEntryRequest _softDeleteEntryRequest;

    public SoftDeleteEntryCommandTests()
    {
        _context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        _command = new SoftDeleteEntryCommand(_context, mockClaimsProvider);

        _softDeleteEntryRequest = new SoftDeleteEntryRequest
        {
            Id = 1,
            DeletionReason = "Deletion reason",
        };
    }

    [Fact]
    public async Task SoftDeleteExistingEntryTwice_ShouldDeleteEntryFromDbSetAndDoNotThrowAtSecondTime()
    {
        var taskEntry = await _context.AddEntityAndSaveAsync(new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID
        });

        var wasDeleted = await _command.ExecuteAsync(_softDeleteEntryRequest);

        Assert.True(wasDeleted);

        var deletedTaskEntry = await _context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.NotNull(deletedTaskEntry);
        Assert.NotNull(deletedTaskEntry.DeletedAtUtc);
        Assert.Equal(_softDeleteEntryRequest.DeletionReason, deletedTaskEntry.DeletionReason);

        var wasDeletedAgain = true;

        // try to delete again
        Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await _command.ExecuteAsync(_softDeleteEntryRequest)));
        Assert.False(wasDeletedAgain);
    }

    [Fact]
    public async Task SoftDeleteNonExistingEntry_ShouldNotThrowException()
    {
        var wasNonExistedDeleted = true;

        const long NON_EXISTING_ID = -1;

        var softDeleteEntryRequest = new SoftDeleteEntryRequest
        {
            Id = NON_EXISTING_ID,
            DeletionReason = "Deletion reason",
        };

        // try to delete a non-existing entry
        Assert.Null(await Record.ExceptionAsync(
            async () => wasNonExistedDeleted = await _command.ExecuteAsync(softDeleteEntryRequest)
        ));
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

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(2, TENANT_ID);

        var command = new SoftDeleteEntryCommand(_context, mockClaimsProvider);

        var wasDeleted = await command.ExecuteAsync(_softDeleteEntryRequest);

        var taskEntryFromDb = await _context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.False(wasDeleted);
        Assert.NotNull(taskEntryFromDb);
        Assert.Null(taskEntryFromDb.DeletedAtUtc);
        Assert.Null(taskEntryFromDb.DeletionReason);
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

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var command = new SoftDeleteEntryCommand(_context, mockClaimsProvider);

        var wasDeleted = await command.ExecuteAsync(_softDeleteEntryRequest);

        var taskEntryFromDb = await _context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.False(wasDeleted);
        Assert.NotNull(taskEntryFromDb);
        Assert.Null(taskEntryFromDb.DeletedAtUtc);
        Assert.Null(taskEntryFromDb.DeletionReason);
    }

    [Fact]
    public async Task SoftDeleteEntryWithMakeUpTimeList_ShouldDeleteEntryWithAllRelatedMakeUpTimeEntry()
    {
        var awayWithMakeUpTimeEntry = await _context.AddEntityAndSaveAsync(new AwayWithMakeUpTimeEntry
        {
            Id = 1,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            MakeUpTimeList =
            [
                new MakeUpTimeEntry
                {
                    Id = 2,
                    RelatedEntryId = 1,
                },
                new MakeUpTimeEntry
                {
                    Id = 3,
                    RelatedEntryId = 1,
                }
            ]
        });

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var command = new SoftDeleteEntryCommand(_context, mockClaimsProvider);

        var wasDeleted = await command.ExecuteAsync(_softDeleteEntryRequest);

        var awayWithMakeUpTimeEntryFromDb = await _context
            .AwayWithMakeUpTimeEntries
            .SingleOrDefaultAsync(x => x.Id == awayWithMakeUpTimeEntry.Id);

        var makeUpTimeEntriesByRelateIdFromDb = await _context
            .MakeUpTimeEntries
            .Where(x => x.RelatedEntryId == awayWithMakeUpTimeEntry.Id)
            .ToListAsync();

        Assert.True(wasDeleted);
        Assert.NotNull(awayWithMakeUpTimeEntryFromDb);
        Assert.NotNull(awayWithMakeUpTimeEntryFromDb.DeletedAtUtc);
        Assert.NotNull(awayWithMakeUpTimeEntryFromDb.DeletionReason);
        Assert.NotEmpty(makeUpTimeEntriesByRelateIdFromDb);
        Assert.NotNull(makeUpTimeEntriesByRelateIdFromDb[0].DeletedAtUtc);
        Assert.NotNull(makeUpTimeEntriesByRelateIdFromDb[0].DeletionReason);
        Assert.NotNull(makeUpTimeEntriesByRelateIdFromDb[1].DeletedAtUtc);
        Assert.NotNull(makeUpTimeEntriesByRelateIdFromDb[1].DeletionReason);
    }
}
