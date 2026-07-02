using Core;
using Core.Entities;
using Xunit;

namespace Application.SharedQueries;

[UnitTest]
public class GetEntryByIdQueryTests
{
    private const long employeeId = 1;
    private const long tenantId = 777;

    [Fact]
    public async Task GetAnotherEmployeesEntryByIdAsync_ShouldNotGetAnotherEmployeesEntry()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(tenantId);

        var taskEntry = new TaskEntry
        {
            Id = 2,
            EmployeeId = employeeId,
            TenantId = tenantId,
        };

        await context.AddEntityAndSaveAsync(taskEntry);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(3, tenantId);

        var getEntryByIdQuery = new GetEntryByIdQuery(context, mockClaimsProvider);

        await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await getEntryByIdQuery.GetAsync<TaskEntry>(taskEntry.Id)
            );
    }

    [Fact]
    public async Task GetAnotherTenantsEntryByIdAsync_ShouldNotGetAnotherTenantsEntry()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(tenantId);

        var taskEntry = new TaskEntry
        {
            Id = 3,
            EmployeeId = employeeId,
            TenantId = 3,
        };

        await context.AddEntityAndSaveAsync(taskEntry);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(employeeId, tenantId);

        var getEntryByIdQuery = new GetEntryByIdQuery(context, mockClaimsProvider);

        await Assert.ThrowsAsync<InvalidOperationException>(
                  async () => await getEntryByIdQuery.GetAsync<TaskEntry>(taskEntry.Id)
              );
    }
}
