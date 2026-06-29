using Application.SharedQueries;
using Core;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Features.Tracking.GetUnwellEntry;

[UnitTest]
public class GetUnwellEntryHandlerTests
{
    protected const long employeeId = 1;
    protected const long tenantId = 777;

    [Fact]
    public async Task GetUnwellEntryHandler_ShouldThrowExceptionIfUnwellEntryNotExistInDb()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(tenantId);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(employeeId, tenantId);

        var getEntryByIdQueryMock = new Mock<IGetEntryByIdQuery>();

        getEntryByIdQueryMock
            .Setup(x => x.GetAsync<UnwellEntry>(It.IsAny<long>()))
            .Returns(Task.FromResult<UnwellEntry>(null));

        var getTaskEntryHandler = new GetUnwellEntryHandler(getEntryByIdQueryMock.Object);

        var nonExistentId = 999;

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => getTaskEntryHandler.HandleAsync(nonExistentId));

        Assert.Equal($"Unwell Entry with id {nonExistentId} does not exist", exception.Message);
    }
}
