using Application.SharedQueries;
using Core;
using Moq;
using Xunit;

namespace Application.Features.Tracking.GetAwayWithMakeUpTimeEntry;

[UnitTest]
public class GetAwayWithMakeUpTimeEntryHandlerTests
{
    protected const long employeeId = 1;
    protected const long tenantId = 777;

    [Fact]
    public async Task GetAwayWithMakeUpTimeEntryHandler_ShouldThrowExceptionIfAwayWithMakeUpTimeEntryNotExistInDb()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(tenantId);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(employeeId, tenantId);

        var getEntryByIdQueryMock = new Mock<IGetEntryByIdQuery>();

        getEntryByIdQueryMock
            .Setup(x => x.GetAsync<AwayWithMakeUpTimeEntry>(It.IsAny<long>()))
            .Returns(Task.FromResult<AwayWithMakeUpTimeEntry>(null!)!);

        var getAwayWithMakeUpTimeEntryHandler = new GetAwayWithMakeUpTimeEntryHandler(getEntryByIdQueryMock.Object);

        var nonExistentId = 999;

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => getAwayWithMakeUpTimeEntryHandler.HandleAsync(nonExistentId));

        Assert.Equal($"Away With Make-up Time Entry with id {nonExistentId} does not exist", exception.Message);
    }
}
