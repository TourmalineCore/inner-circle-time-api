using Application.TestsConfig;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Queries;

[UnitTest]
public class GetEntriesByPeriodQueryTests
{
    private const long EMPLOYEE_ID = 1;
    private const long TENANT_ID = 777;

    [Fact]
    public async Task GetEntriesByPeriodAsync_ShouldReturnEntriesByPeriodFromDbSet()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);
        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, mockClaimsProvider.Object);

        var taskEntry1 = new TaskEntry
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        var taskEntry2 = new TaskEntry
        {
            Id = 12,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 27, 10, 0, 0),
        };

        var taskEntry3 = new TaskEntry
        {
            Id = 13,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 10, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 10, 27, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(taskEntry1);
        await context.AddEntityAndSaveAsync(taskEntry2);
        await context.AddEntityAndSaveAsync(taskEntry3);

        var result = await getEntriesByPeriodQuery
            .GetByPeriodAsync<TaskEntry>(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == taskEntry1.Id);
        Assert.Contains(result, x => x.Id == taskEntry2.Id);
        Assert.DoesNotContain(result, x => x.Id == taskEntry3.Id);
    }

    [Fact]
    public async Task GetAnotherEmployeesEntriesByPeriodAsync_ShouldNotGetAnotherEmployeesEntries()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var taskEntry = new TaskEntry
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(taskEntry);

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(3);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, mockClaimsProvider.Object);

        var result = await getEntriesByPeriodQuery
            .GetByPeriodAsync<TaskEntry>(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.DoesNotContain(result, x => x.Id == taskEntry.Id);
    }

    [Fact]
    public async Task GetAnotherTenantsEntriesByPeriodAsync_ShouldNotGetAnotherTenantsEntries()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var taskEntry = new TaskEntry
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = 3,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(taskEntry);

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, mockClaimsProvider.Object);

        var result = await getEntriesByPeriodQuery
            .GetByPeriodAsync<TaskEntry>(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.DoesNotContain(result, x => x.Id == taskEntry.Id);
    }
}
