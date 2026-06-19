using Core;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;

[UnitTest]
public class UpdateAwayWithMakeUpTimeEntryCommandTests
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    [Fact]
    public async Task UpdateAwayWithMakeUpTimeEntryAsync_ShouldThrowExceptionIfAwayWithMakeUpTimeNotExistInDb()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var awayWithMakeUpTimeEntry = await context.AddEntityAndSaveAsync(
            new AwayWithMakeUpTimeEntry
            {
                Id = 1,
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
            });

        var updateAwayWithMakeUpTimeEntryRequest = new UpdateAwayWithMakeUpTimeEntryRequest
        {
            Id = 999,
            StartTime = new DateTime(2025, 11, 25, 12, 0, 0),
            EndTime = new DateTime(2025, 11, 25, 11, 0, 0),
            Description = "Description",
            MakeUpTimeList = [
                new MakeUpTimeEntryDto
                {
                    StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 12, 0, 0)
                }
            ]
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => updateAwayWithMakeUpTimeEntryCommand.ExecuteAsync(updateAwayWithMakeUpTimeEntryRequest));

        Assert.Equal($"Away With Make Up Time Entry with id {updateAwayWithMakeUpTimeEntryRequest.Id} does not exist", exception.Message);
    }

    [Fact]
    public async Task UpdateAwayWithMakeUpTimeEntryAsync_ShouldNotUpdateMakeUpTimeEntryIfItsTimeHasNotBeenUpdated()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var awayWithMakeUpTimeEntry = await context.AddEntityAndSaveAsync(
            new AwayWithMakeUpTimeEntry
            {
                Id = 1,
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                MakeUpTimeList =
                [
                    new MakeUpTimeEntry
                    {
                        Id = 2,
                        EmployeeId = EMPLOYEE_ID,
                        TenantId = TENANT_ID,
                        RelatedEntryId = 1,
                        StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 12, 0, 0)
                    }
                ]
            });

        var updateAwayWithMakeUpTimeEntryRequest = new UpdateAwayWithMakeUpTimeEntryRequest
        {
            Id = awayWithMakeUpTimeEntry.Id,
            StartTime = new DateTime(2025, 11, 25, 12, 0, 0),
            EndTime = new DateTime(2025, 11, 25, 11, 0, 0),
            Description = "New Description",
            MakeUpTimeList = [
                new MakeUpTimeEntryDto
                {
                    StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 12, 0, 0)
                }
            ]
        };

        await updateAwayWithMakeUpTimeEntryCommand.ExecuteAsync(updateAwayWithMakeUpTimeEntryRequest);

        var makeUpTimeEntry = await context
            .MakeUpTimeEntries
            .SingleOrDefaultAsync(x => x.RelatedEntryId == updateAwayWithMakeUpTimeEntryRequest.Id);

        Assert.NotNull(makeUpTimeEntry);
        Assert.Equal(2, makeUpTimeEntry.Id);
    }

    [Fact]
    public async Task UpdateAwayWithMakeUpTimeEntryAsync_ShouldUpdateOnlyThoseMakeUpTimeEntriesWhoseTimeHasBeenUpdated()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var awayWithMakeUpTimeEntry = await context.AddEntityAndSaveAsync(
            new AwayWithMakeUpTimeEntry
            {
                Id = 1,
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                MakeUpTimeList =
                [
                    new MakeUpTimeEntry
                    {
                        Id = 2,
                        EmployeeId = EMPLOYEE_ID,
                        TenantId = TENANT_ID,
                        RelatedEntryId = 1,
                        StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 12, 0, 0)
                    },
                    new MakeUpTimeEntry
                    {
                        Id = 3,
                        EmployeeId = EMPLOYEE_ID,
                        TenantId = TENANT_ID,
                        RelatedEntryId = 1,
                        StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 14, 0, 0)
                    }
                ]
            });

        var updateAwayWithMakeUpTimeEntryRequest = new UpdateAwayWithMakeUpTimeEntryRequest
        {
            Id = awayWithMakeUpTimeEntry.Id,
            StartTime = new DateTime(2025, 11, 25, 12, 0, 0),
            EndTime = new DateTime(2025, 11, 25, 11, 0, 0),
            Description = "New Description",
            MakeUpTimeList = [
                new MakeUpTimeEntryDto
                {
                    StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 12, 0, 0)
                },
                    new MakeUpTimeEntryDto
                {
                    StartTime = new DateTime(2025, 11, 25, 13, 0, 0),
                    EndTime = new DateTime(2025, 11, 25, 14, 0, 0)
                }
            ]
        };

        await updateAwayWithMakeUpTimeEntryCommand.ExecuteAsync(updateAwayWithMakeUpTimeEntryRequest);

        var makeUpTimeEntries = await context
            .MakeUpTimeEntries
            .Where(x => x.RelatedEntryId == updateAwayWithMakeUpTimeEntryRequest.Id)
            .ToListAsync();

        Assert.NotEmpty(makeUpTimeEntries);
        Assert.Equal(2, makeUpTimeEntries.Count);
        Assert.Contains(makeUpTimeEntries, x => x.Id == 2);
        Assert.Contains(makeUpTimeEntries, x => x.Id == 4);
    }
}
