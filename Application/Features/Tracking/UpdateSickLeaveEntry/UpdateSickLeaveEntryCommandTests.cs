using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.UpdateSickLeaveEntry;

[IntegrationTest]
public class UpdateSickLeaveEntryCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task UpdateSickLeaveEntryAsync_ShouldUpdateDbWithCorrectStartTimeAndEndTime()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var existingSickLeaveEntry = await SaveEntityAsync(context, new SickLeaveEntry
        {
            EmployeeId = EMPLOYEE_ID,
            StartTime = new DateTime(2026, 7, 13, 0, 0, 0),
            EndTime = new DateTime(2026, 7, 17, 0, 0, 0)
        });

        var updateSickLeaveEntryCommand = new UpdateSickLeaveEntryCommand(context, mockClaimsProvider);

        var updateSickLeaveEntryRequest = new UpdateSickLeaveEntryRequest
        {
            Id = existingSickLeaveEntry.Id,
            Period = new PeriodDto
            {
                StartDate = new DateOnly(2026, 7, 12),
                EndDate = new DateOnly(2026, 7, 20)
            }
        };

        await updateSickLeaveEntryCommand.ExecuteAsync(updateSickLeaveEntryRequest);

        var sickLeaveEntryFromDb = await context
            .SickLeaveEntries
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == existingSickLeaveEntry.Id);

        Assert.NotNull(sickLeaveEntryFromDb);
        Assert.Equal(new DateTime(2026, 7, 12, 0, 0, 0), sickLeaveEntryFromDb.StartTime);
        Assert.Equal(new DateTime(2026, 7, 21, 0, 0, 0), sickLeaveEntryFromDb.EndTime);
    }
}
