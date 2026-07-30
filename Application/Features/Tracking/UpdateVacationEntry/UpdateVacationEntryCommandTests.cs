using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.UpdateVacationEntry;

[IntegrationTest]
public class UpdateVacationEntryCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task UpdateVacationEntryAsync_ShouldUpdateIsUnpaidFromFalseToTrue()
    {
        var context = CreateTenantDbContext();

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var existingVacationEntry = await SaveEntityAsync(context, new VacationEntry
        {
            EmployeeId = EMPLOYEE_ID,
            StartTime = new DateTime(2026, 7, 13, 0, 0, 0),
            EndTime = new DateTime(2026, 7, 17, 0, 0, 0),
            IsUnpaid = false
        });

        var updateVacationEntryCommand = new UpdateVacationEntryCommand(context, mockClaimsProvider);

        var updateVacationEntryRequest = new UpdateVacationEntryRequest
        {
            Id = existingVacationEntry.Id,
            Period = new PeriodDto
            {
                StartDate = new DateOnly(2026, 7, 13),
                EndDate = new DateOnly(2026, 7, 16)
            },
            IsUnpaid = true
        };

        await updateVacationEntryCommand.ExecuteAsync(updateVacationEntryRequest);

        var vacationEntryFromDb = await context
            .VacationEntries
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == existingVacationEntry.Id);

        Assert.NotNull(vacationEntryFromDb);
        Assert.True(vacationEntryFromDb.IsUnpaid);
    }
}
