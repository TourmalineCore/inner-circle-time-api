using Application.TestsConfig;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Queries;

[UnitTest]
public class GetWorkEntriesByPeriodQueryTests
{
    private const long EMPLOYEE_ID = 1;
    private const long TENANT_ID = 777;

    [Fact]
    public async Task GetWorkEntriesByPeriodAsync_ShouldReturnWorkEntriesByPeriodFromDbSet()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);
        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var getWorkEntriesByPeriodQuery = new GetWorkEntriesByPeriodQuery(context, mockClaimsProvider.Object);

        var workEntry1 = new WorkEntry
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        var workEntry2 = new WorkEntry
        {
            Id = 12,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 27, 10, 0, 0),
        };

        var workEntry3 = new WorkEntry
        {
            Id = 13,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 10, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 10, 27, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(workEntry1);
        await context.AddEntityAndSaveAsync(workEntry2);
        await context.AddEntityAndSaveAsync(workEntry3);

        var result = await getWorkEntriesByPeriodQuery
            .GetByPeriodAsync<WorkEntry>(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == workEntry1.Id);
        Assert.Contains(result, x => x.Id == workEntry2.Id);
        Assert.DoesNotContain(result, x => x.Id == workEntry3.Id);
    }

    [Fact]
    public async Task GetAnotherEmployeesWorkEntriesByPeriodAsync_ShouldNotGetAnotherEmployeesWorkEntries()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var workEntry = new WorkEntry
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(workEntry);

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(3);

        var getWorkEntriesByPeriodQuery = new GetWorkEntriesByPeriodQuery(context, mockClaimsProvider.Object);

        var result = await getWorkEntriesByPeriodQuery
            .GetByPeriodAsync<WorkEntry>(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.DoesNotContain(result, x => x.Id == workEntry.Id);
    }

    [Fact]
    public async Task GetAnotherTenantsWorkEntriesByPeriodAsync_ShouldNotGetAnotherTenantsWorkEntries()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var workEntry = new WorkEntry
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = 3,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(workEntry);

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var getWorkEntriesByPeriodQuery = new GetWorkEntriesByPeriodQuery(context, mockClaimsProvider.Object);

        var result = await getWorkEntriesByPeriodQuery
            .GetByPeriodAsync<WorkEntry>(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.DoesNotContain(result, x => x.Id == workEntry.Id);
    }
}
