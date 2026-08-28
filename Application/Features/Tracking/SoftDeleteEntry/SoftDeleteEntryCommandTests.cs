using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.SoftDeleteEntry;

[IntegrationTest]
public class SoftDeleteEntryCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task SoftDeleteExistingEntryTwice_ShouldDeleteEntryFromDbSetAndDoNotThrowAtSecondTime()
    {
        var context = CreateTenantDbContext();
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var softDeleteEntryCommand = new SoftDeleteEntryCommand(context, mockClaimsProvider);

        var taskEntry = await AddEntityAndSaveAsync(
            context,
            new TaskEntry
            {
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                StartTime = new DateTime(2025, 11, 23, 13, 0, 0),
                EndTime = new DateTime(2025, 11, 23, 14, 0, 0),
            }
        );

        var softDeleteEntryRequest = new SoftDeleteEntryRequest
        {
            Id = taskEntry.Id,
            DeletionReason = "Deletion reason",
        };

        var wasDeleted = await softDeleteEntryCommand.ExecuteAsync(softDeleteEntryRequest);

        Assert.True(wasDeleted);

        var deletedTaskEntry = await context
            .TaskEntries
            .SingleOrDefaultAsync(x => x.Id == taskEntry.Id);

        Assert.NotNull(deletedTaskEntry);
        Assert.NotNull(deletedTaskEntry.DeletedAtUtc);
        Assert.Equal(softDeleteEntryRequest.DeletionReason, deletedTaskEntry.DeletionReason);

        var wasDeletedAgain = true;

        // try to delete again
        Assert.Null(await Record.ExceptionAsync(async () => wasDeletedAgain = await softDeleteEntryCommand.ExecuteAsync(softDeleteEntryRequest)));
        Assert.False(wasDeletedAgain);
    }

    [Fact]
    public async Task SoftDeleteNonExistingEntry_ShouldNotThrowException()
    {
        var context = CreateTenantDbContext();
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var softDeleteEntryCommand = new SoftDeleteEntryCommand(context, mockClaimsProvider);

        var wasNonExistedDeleted = true;

        const long NON_EXISTING_ID = -1;

        var softDeleteEntryRequest = new SoftDeleteEntryRequest
        {
            Id = NON_EXISTING_ID,
            DeletionReason = "Deletion reason",
        };

        // try to delete a non-existing entry
        Assert.Null(await Record.ExceptionAsync(
            async () => wasNonExistedDeleted = await softDeleteEntryCommand.ExecuteAsync(softDeleteEntryRequest)
        ));
        Assert.False(wasNonExistedDeleted);
    }

    [Fact]
    public async Task SoftDeleteAnotherEmployeesEntry_ShouldNotDeleteAnotherEmployeesEntryFromDb()
    {
        var context = CreateTenantDbContext();

        var taskEntry = await AddEntityAndSaveAsync(
            context,
            new TaskEntry
            {
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                StartTime = new DateTime(2025, 11, 23, 13, 0, 0),
                EndTime = new DateTime(2025, 11, 23, 14, 0, 0),
            }
        );

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(2, TENANT_ID);

        var softDeleteEntryCommand = new SoftDeleteEntryCommand(context, mockClaimsProvider);

        var softDeleteEntryRequest = new SoftDeleteEntryRequest
        {
            Id = taskEntry.Id,
            DeletionReason = "Deletion reason",
        };

        var wasDeleted = await softDeleteEntryCommand.ExecuteAsync(softDeleteEntryRequest);

        var taskEntryFromDb = await context
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
        var context = CreateTenantDbContext();

        var taskEntry = await AddEntityAndSaveAsync(
            context,
            new TaskEntry
            {
                EmployeeId = EMPLOYEE_ID,
                // To check the tenant isolation, you must specify a TenantId other than 777,
                // since in the implementation of CreateTenantDbContext filters by TenantId = 777
                TenantId = 2,
                StartTime = new DateTime(2025, 11, 23, 13, 0, 0),
                EndTime = new DateTime(2025, 11, 23, 14, 0, 0),
            }
        );

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var command = new SoftDeleteEntryCommand(context, mockClaimsProvider);

        var softDeleteEntryRequest = new SoftDeleteEntryRequest
        {
            Id = taskEntry.Id,
            DeletionReason = "Deletion reason",
        };

        var wasDeleted = await command.ExecuteAsync(softDeleteEntryRequest);

        var taskEntryFromDb = await context
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
        var context = CreateTenantDbContext();

        var awayWithMakeUpTimeEntry = await AddEntityAndSaveAsync(
            context,
            new AwayWithMakeUpTimeEntry
            {
                Id = 1,
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                StartTime = new DateTime(2025, 11, 23, 13, 0, 0),
                EndTime = new DateTime(2025, 11, 23, 14, 0, 0),
                MakeUpTimeList =
                [
                    new MakeUpTimeEntry
                    {
                        Id = 2,
                        RelatedEntryId = 1,
                        StartTime = new DateTime(2025, 11, 24, 17, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 18, 0, 0),
                    },
                    new MakeUpTimeEntry
                    {
                        Id = 3,
                        RelatedEntryId = 1,
                        StartTime = new DateTime(2025, 11, 25, 17, 0, 0),
                        EndTime = new DateTime(2025, 11, 25, 18, 0, 0),
                    }
                ]
            }
        );

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var command = new SoftDeleteEntryCommand(context, mockClaimsProvider);

        var softDeleteEntryRequest = new SoftDeleteEntryRequest
        {
            Id = awayWithMakeUpTimeEntry.Id,
            DeletionReason = "Deletion reason",
        };

        var wasDeleted = await command.ExecuteAsync(softDeleteEntryRequest);

        var awayWithMakeUpTimeEntryFromDb = await context
            .AwayWithMakeUpTimeEntries
            .SingleOrDefaultAsync(x => x.Id == awayWithMakeUpTimeEntry.Id);

        var makeUpTimeEntriesByRelateIdFromDb = await context
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
