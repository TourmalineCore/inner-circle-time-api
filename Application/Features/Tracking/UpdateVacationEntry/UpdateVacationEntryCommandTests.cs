using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.UpdateVacationEntry;

[IntegrationTest]
public class UpdateVacationEntryCommandTests : IntegrationTestBase
{
    [Fact]
    // we decided to keep this check of IsUnpaid toggling as a separate test rather than checking it together with the period update in the related e2e test to keep an example and explanation of AsNoTracking trick
    // this trick covers this case of C# tests: create, update, and read to verify using real db
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
                EndDate = new DateOnly(2026, 7, 16) // it's valid that it's one day before the date in EndTime
            },
            IsUnpaid = true
        };

        await updateVacationEntryCommand.ExecuteAsync(updateVacationEntryRequest);

        var vacationEntryFromDb = await context
            .VacationEntries
            // before that request we already have this vacation entry in EF Core context, since we track its changes
            // if we read it again without AsNoTracking it won't execute SQL but just return the cached instance of vacation entry from the context
            // adding AsNoTracking we force EF Core to make SQL ignoring its internal cache
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == existingVacationEntry.Id);

        Assert.NotNull(vacationEntryFromDb);
        Assert.True(vacationEntryFromDb.IsUnpaid);
    }
}
