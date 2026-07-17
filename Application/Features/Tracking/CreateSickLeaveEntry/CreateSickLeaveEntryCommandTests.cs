using Core;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Features.Tracking.CreateSickLeaveEntry;


[UnitTest]
public class CreateSickLeaveEntryCommandTests
{
    private long tenantId = 777;

    [Fact]
    public async Task CreateSickLeaveEntryAsync_ShouldSaveInDbWithCorrectStartTimeAndEndTime()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(tenantId);
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(1, tenantId);

        var createSickLeaveEntryCommand = new CreateSickLeaveEntryCommand(context, mockClaimsProvider);

        var сreateSickLeaveEntryRequest = new CreateSickLeaveEntryRequest
        {
            Period = new PeriodDto
            {
                StartDate = new DateOnly(2026, 7, 13),
                EndDate = new DateOnly(2026, 7, 17)
            }
        };

        var newSickLeaveEntryId = await createSickLeaveEntryCommand.ExecuteAsync(сreateSickLeaveEntryRequest);

        var sickLeaveEntryFromDb = await context
            .SickLeaveEntries
            .SingleOrDefaultAsync(x => x.Id == newSickLeaveEntryId);

        Assert.NotNull(sickLeaveEntryFromDb);
        Assert.Equal(new DateTime(2026, 7, 13, 0, 0, 0), sickLeaveEntryFromDb.StartTime);
        Assert.Equal(new DateTime(2026, 7, 18, 0, 0, 0), sickLeaveEntryFromDb.EndTime);
    }
}
