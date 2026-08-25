using Core;
using Xunit;

namespace Application.Features.Tracking.UpdateAwayWithMakeUpTimeEntry;

[UnitTest]
public class UpdateAwayWithMakeUpTimeEntryHandlerTests
{
    protected const long employeeId = 1;
    protected const long tenantId = 777;

    [Fact]
    public async Task UpdateAwayWithMakeUpTimeEntryHandler_ShouldThrowExceptionIfMakeUpTotalTimeDoesNotMatchWithRelatedEntryPeriod()
    {
        var (ctx, conn) = await TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTestsAsync(tenantId);

        await using var context = ctx;
        await using var connection = conn;

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(employeeId, tenantId);

        var updateAwayWithMakeUpTimeEntryCommand = new UpdateAwayWithMakeUpTimeEntryCommand(context, mockClaimsProvider);

        var updateAwayWithMakeUpTimeEntryHandler = new UpdateAwayWithMakeUpTimeEntryHandler(updateAwayWithMakeUpTimeEntryCommand);

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


        var exception = await Assert.ThrowsAsync<TimeDoesNotMatchException>(
                async () => await updateAwayWithMakeUpTimeEntryHandler.HandleAsync(999, updateAwayWithMakeUpTimeEntryRequest)
            );

        Assert.Equal("Total make-up time must equal your away time. Please check and adjust your entries.", exception.Message);
    }
}
