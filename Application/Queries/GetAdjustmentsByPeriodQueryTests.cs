using Application.TestsConfig;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Queries;

[UnitTest]
public class GetAdjustmentsByPeriodQueryTests
{
    private const long EMPLOYEE_ID = 1;
    private const long TENANT_ID = 777;

    [Fact]
    public async Task GetAdjustmentsByPeriodAsync_ShouldReturnAdjustmentsByPeriodFromDbSet()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);
        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var getAdjustmentsByPeriodQuery = new GetAdjustmentsByPeriodQuery(context, mockClaimsProvider.Object);

        var adjustment1 = new Adjustment
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        var adjustment2 = new Adjustment
        {
            Id = 12,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 27, 10, 0, 0),
        };

        var adjustment3 = new Adjustment
        {
            Id = 13,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 10, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 10, 27, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(adjustment1);
        await context.AddEntityAndSaveAsync(adjustment2);
        await context.AddEntityAndSaveAsync(adjustment3);

        var result = await getAdjustmentsByPeriodQuery
            .GetByPeriodAsync(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == adjustment1.Id);
        Assert.Contains(result, x => x.Id == adjustment2.Id);
        Assert.DoesNotContain(result, x => x.Id == adjustment3.Id);
    }

    [Fact]
    public async Task GetAnotherEmployeesAdjustmentsByPeriodAsync_ShouldNotGetAnotherEmployeesAdjustments()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var adjustment = new Adjustment
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(adjustment);

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(3);

        var getAdjustmentsByPeriodQuery = new GetAdjustmentsByPeriodQuery(context, mockClaimsProvider.Object);

        var result = await getAdjustmentsByPeriodQuery
            .GetByPeriodAsync(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.DoesNotContain(result, x => x.Id == adjustment.Id);
    }

    [Fact]
    public async Task GetAnotherTenantsAdjustmentsByPeriodAsync_ShouldNotGetAnotherTenantsAdjustments()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var adjustment = new Adjustment
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            TenantId = 3,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(adjustment);

        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(x => x.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var getAdjustmentsByPeriodQuery = new GetAdjustmentsByPeriodQuery(context, mockClaimsProvider.Object);

        var result = await getAdjustmentsByPeriodQuery
            .GetByPeriodAsync(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.DoesNotContain(result, x => x.Id == adjustment.Id);
    }
}
