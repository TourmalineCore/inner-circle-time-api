using Core;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;

[IntegrationTest]
public class UpdateAwayWithMakeUpTimeEntryCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task UpdateAwayWithMakeUpTimeEntryAsync_ShouldThrowExceptionIfAwayWithMakeUpTimeNotExistInDb()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var awayWithMakeUpTimeEntry = await AddEntityAndSaveAsync(
            context,
            new AwayWithMakeUpTimeEntry
            {
                Id = 1,
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                StartTime = new DateTime(2025, 11, 23, 10, 0, 0),
                EndTime = new DateTime(2025, 11, 23, 11, 0, 0),
            }
        );

        var updateAwayWithMakeUpTimeEntryRequest = new UpdateAwayWithMakeUpTimeEntryRequest
        {
            Id = 999,
            StartTime = new DateTime(2025, 11, 23, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 23, 12, 0, 0),
            Description = "Description",
            MakeUpTimeList = [
                new CreateOrUpdateMakeUpTimeEntryDto
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
        var context = CreateTenantDbContext();

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var awayWithMakeUpTimeEntry = await AddEntityAndSaveAsync(
            context,
            new AwayWithMakeUpTimeEntry
            {
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                StartTime = new DateTime(2025, 11, 23, 11, 0, 0),
                EndTime = new DateTime(2025, 11, 23, 12, 0, 0),
                MakeUpTimeList =
                [
                    new MakeUpTimeEntry
                    {
                        EmployeeId = EMPLOYEE_ID,
                        TenantId = TENANT_ID,
                        StartTime = new DateTime(2025, 11, 24, 17, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 18, 0, 0)
                    }
                ]
            }
        );

        var updateAwayWithMakeUpTimeEntryRequest = new UpdateAwayWithMakeUpTimeEntryRequest
        {
            Id = awayWithMakeUpTimeEntry.Id,
            StartTime = new DateTime(2025, 11, 23, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 23, 12, 0, 0),
            Description = "New Description",
            MakeUpTimeList = [
                new CreateOrUpdateMakeUpTimeEntryDto
                {
                    StartTime = new DateTime(2025, 11, 24, 17, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 18, 0, 0)
                }
            ]
        };

        await updateAwayWithMakeUpTimeEntryCommand.ExecuteAsync(updateAwayWithMakeUpTimeEntryRequest);

        var makeUpTimeEntry = await context
            .MakeUpTimeEntries
            .SingleOrDefaultAsync(x => x.RelatedEntryId == updateAwayWithMakeUpTimeEntryRequest.Id);

        Assert.NotNull(makeUpTimeEntry);
        Assert.Equal(awayWithMakeUpTimeEntry.MakeUpTimeList[0].Id, makeUpTimeEntry.Id);
    }

    [Fact]
    public async Task UpdateAwayWithMakeUpTimeEntryAsync_ShouldUpdateOnlyThoseMakeUpTimeEntriesWhoseTimeHasBeenUpdated()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var awayWithMakeUpTimeEntry = await AddEntityAndSaveAsync(
            context,
            new AwayWithMakeUpTimeEntry
            {
                EmployeeId = EMPLOYEE_ID,
                TenantId = TENANT_ID,
                StartTime = new DateTime(2025, 11, 23, 11, 0, 0),
                EndTime = new DateTime(2025, 11, 23, 12, 0, 0),
                MakeUpTimeList =
                [
                    new MakeUpTimeEntry
                    {
                        EmployeeId = EMPLOYEE_ID,
                        TenantId = TENANT_ID,
                        StartTime = new DateTime(2025, 11, 24, 17, 0, 0),
                        EndTime = new DateTime(2025, 11, 24, 18, 0, 0)
                    },
                    new MakeUpTimeEntry
                    {
                        EmployeeId = EMPLOYEE_ID,
                        TenantId = TENANT_ID,
                        StartTime = new DateTime(2025, 11, 25, 17, 0, 0),
                        EndTime = new DateTime(2025, 11, 25, 18, 0, 0)
                    }
                ]
            }
        );

        var updateAwayWithMakeUpTimeEntryRequest = new UpdateAwayWithMakeUpTimeEntryRequest
        {
            Id = awayWithMakeUpTimeEntry.Id,
            StartTime = new DateTime(2025, 11, 25, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 25, 12, 0, 0),
            Description = "New Description",
            MakeUpTimeList = [
                new CreateOrUpdateMakeUpTimeEntryDto
                {
                    StartTime = new DateTime(2025, 11, 24, 17, 0, 0),
                    EndTime = new DateTime(2025, 11, 24, 18, 0, 0)
                },
                new CreateOrUpdateMakeUpTimeEntryDto
                {
                    StartTime = new DateTime(2025, 11, 26, 17, 0, 0),
                    EndTime = new DateTime(2025, 11, 26, 18, 0, 0)
                }
            ]
        };

        // saving the make-up time entry ID before executing the command, since the awayWithMakeUpTimeEntry object may mutate after the update
        // therefore, we cannot simply refer to awayWithMakeUpTimeEntry.MakeUpTimeList[0].Id in the verification
        var firstMakeUpTimeEntryId = awayWithMakeUpTimeEntry.MakeUpTimeList[0].Id;
        var secondMakeUpTimeEntryId = awayWithMakeUpTimeEntry.MakeUpTimeList[1].Id;

        await updateAwayWithMakeUpTimeEntryCommand.ExecuteAsync(updateAwayWithMakeUpTimeEntryRequest);

        var makeUpTimeEntries = await context
            .MakeUpTimeEntries
            .Where(x => x.RelatedEntryId == updateAwayWithMakeUpTimeEntryRequest.Id)
            .ToListAsync();

        Assert.NotEmpty(makeUpTimeEntries);
        Assert.Equal(2, makeUpTimeEntries.Count);
        Assert.Contains(makeUpTimeEntries, x => x.Id == firstMakeUpTimeEntryId);
        Assert.DoesNotContain(makeUpTimeEntries, x => x.Id == secondMakeUpTimeEntryId);
    }
}
