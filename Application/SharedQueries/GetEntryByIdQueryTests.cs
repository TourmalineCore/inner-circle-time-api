using Core;
using Core.Entities;
using Xunit;

namespace Application.SharedQueries;

[IntegrationTest]
public class GetEntryByIdQueryTests : IntegrationTestBase
{
    [Fact]
    public async Task GetAnotherEmployeesEntryByIdAsync_ShouldNotGetAnotherEmployeesEntry()
    {
        var context = CreateTenantDbContext();

        var taskEntry = new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 23, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 23, 12, 0, 0),
        };

        await AddEntityAndSaveAsync(context, taskEntry);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(3, TENANT_ID);

        var getEntryByIdQuery = new GetEntryByIdQuery(context, mockClaimsProvider);

        await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await getEntryByIdQuery.GetAsync<TaskEntry>(taskEntry.Id)
            );
    }

    [Fact]
    public async Task GetAnotherTenantsEntryByIdAsync_ShouldNotGetAnotherTenantsEntry()
    {
        var context = CreateTenantDbContext();

        var taskEntry = new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = 3,
            StartTime = new DateTime(2025, 11, 23, 11, 0, 0),
            EndTime = new DateTime(2025, 11, 23, 12, 0, 0),
        };

        await AddEntityAndSaveAsync(context, taskEntry);

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

        var getEntryByIdQuery = new GetEntryByIdQuery(context, mockClaimsProvider);

        await Assert.ThrowsAsync<InvalidOperationException>(
                  async () => await getEntryByIdQuery.GetAsync<TaskEntry>(taskEntry.Id)
              );
    }
}
