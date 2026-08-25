using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.UpdateVacationEntry;

[UnitTest]
public class UpdateVacationEntryCommandTests
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    [Fact]
    public async Task UpdateVacationEntryAsync_ShouldUpdateIsUnpaidFromFalseToTrue()
    {
        var (context, connection) = await TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTestsAsync(TENANT_ID);

        await using (context)
        await using (connection)
        {
            var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

            var existingVacationEntry = await context.AddEntityAndSaveAsync(
                new VacationEntry
                {
                    TenantId = TENANT_ID,
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
}
