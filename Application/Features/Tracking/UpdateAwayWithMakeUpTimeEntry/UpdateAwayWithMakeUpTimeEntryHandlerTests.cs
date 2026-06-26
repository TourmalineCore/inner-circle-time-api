using Core;
using Xunit;

namespace Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;

[UnitTest]
public class UpdateAwayWithMakeUpTimeEntryHandlerTests
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    [Fact]
    public async Task UpdateAwayWithMakeUpTimeEntryHandler_ShouldThrowExceptionIfMakeUpTotalTimeDoesNotMatchWithRelatedEntryPeriod()
    {
        var updateAwayWithMakeUpTimeEntryRequest = new UpdateAwayWithMakeUpTimeEntryRequest
        {
            StartTime = new DateTime(2026, 11, 24, 10, 0, 0),
            EndTime = new DateTime(2026, 11, 24, 12, 0, 0),
            Description = "Description",
            MakeUpTimeList = [
                new MakeUpTimeEntryDto
                    {
                        StartTime = new DateTime(2026, 11, 24, 17, 0, 0),
                        EndTime = new DateTime(2026, 11, 24, 18, 0, 0),
                    }
                ]
        };

        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var updateAwayWithMakeUpTimeEntryHandler = new UpdateAwayWithMakeUpTimeEntryHandler(updateAwayWithMakeUpTimeEntryCommand);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
                async () => await updateAwayWithMakeUpTimeEntryHandler.HandleAsync(999, updateAwayWithMakeUpTimeEntryRequest)
            );

        Assert.Equal("Total make-up time must equal your away time. Please check and adjust your entries.", exception.Message);
    }
}
