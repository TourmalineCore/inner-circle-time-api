using Application.SharedQueries;
using Core;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Features.Tracking.GetTaskEntry;

[UnitTest]
public class GetTaskEntryHandlerTests
{
    protected const long employeeId = 1;
    protected const long tenantId = 777;

    [Fact]
    public async Task GetTaskEntryHandler_ShouldThrowExceptionIfTaskEntryNotExistInDb()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(tenantId);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(employeeId, tenantId);

        var getEntryByIdQueryMock = new Mock<IGetEntryByIdQuery>();

        getEntryByIdQueryMock
            .Setup(x => x.GetAsync<TaskEntry>(It.IsAny<long>()))
            .Returns(Task.FromResult<TaskEntry>(null!)!);

        var getTaskEntryHandler = new GetTaskEntryHandler(getEntryByIdQueryMock.Object);

        var nonExistentId = 999;

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => getTaskEntryHandler.HandleAsync(nonExistentId));

        Assert.Equal($"Task Entry with id {nonExistentId} does not exist", exception.Message);
    }
}
