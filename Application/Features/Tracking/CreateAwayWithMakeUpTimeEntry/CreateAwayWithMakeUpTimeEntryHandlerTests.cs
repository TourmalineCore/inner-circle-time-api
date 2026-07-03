using Core;
using Xunit;

namespace Application.Features.Tracking.CreateAwayWithMakeUpTimeEntry;

[UnitTest]
public class CreateAwayWithMakeUpTimeEntryHandlerTests
{
    protected const long EMPLOYEE_ID = 1;
    protected const long TENANT_ID = 777;

    [Fact]
    public async Task CreateAwayWithMakeUpTimeEntryHandler_ShouldThrowExceptionIfMakeUpTotalTimeDoesNotMatchWithRelatedEntryPeriod()
    {
        var createAwayWithMakeUpTimeEntryRequest = new CreateAwayWithMakeUpTimeEntryRequest
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

        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);
        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var createAwayWithMakeUpTimeEntryCommand = new CreateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var createAwayWithMakeUpTimeEntryHandler = new CreateAwayWithMakeUpTimeEntryHandler(createAwayWithMakeUpTimeEntryCommand);

        var exception = await Assert.ThrowsAsync<TimeDoesNotMatchException>(
                async () => await createAwayWithMakeUpTimeEntryHandler.HandleAsync(createAwayWithMakeUpTimeEntryRequest)
            );

        Assert.Equal("Total make-up time must equal your away time. Please check and adjust your entries.", exception.Message);
    }
}
