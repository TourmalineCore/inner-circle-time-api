using Core;
using Xunit;

namespace Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;

[IntegrationTest]
public class UpdateAwayWithMakeUpTimeEntryHandlerTests : IntegrationTestBase
{
    [Fact]
    public async Task UpdateAwayWithMakeUpTimeEntryHandler_ShouldThrowExceptionIfMakeUpTotalTimeDoesNotMatchWithRelatedEntryPeriod()
    {
        var updateAwayWithMakeUpTimeEntryRequest = new UpdateAwayWithMakeUpTimeEntryRequest
        {
            StartTime = new DateTime(2026, 11, 24, 10, 0, 0),
            EndTime = new DateTime(2026, 11, 24, 12, 0, 0),
            Description = "Description",
            MakeUpTimeList = [
                new CreateOrUpdateMakeUpTimeEntryDto
                    {
                        StartTime = new DateTime(2026, 11, 24, 17, 0, 0),
                        EndTime = new DateTime(2026, 11, 24, 18, 0, 0),
                    }
                ]
        };

        var context = CreateTenantDbContext();
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var updateAwayWithMakeUpTimeEntryHandler = new UpdateAwayWithMakeUpTimeEntryHandler(updateAwayWithMakeUpTimeEntryCommand);

        var exception = await Assert.ThrowsAsync<TimeDoesNotMatchException>(
                async () => await updateAwayWithMakeUpTimeEntryHandler.HandleAsync(999, updateAwayWithMakeUpTimeEntryRequest)
            );

        Assert.Equal("Total make-up time must equal your away time. Please check and adjust your entries.", exception.Message);
    }
}
