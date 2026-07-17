using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.Tracking.GetEntriesByPeriod;

[UnitTest]
public class GetEntriesByPeriodQueryTests
{
    private const long EMPLOYEE_ID = 1;
    private const long TENANT_ID = 777;

    private IClaimsProvider _mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

    [Fact]
    public async Task GetEntriesByPeriodAsync_ShouldReturnEntriesByPeriodFromDbSet()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

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

        var mockClaimsProvider = MockClaimsProviderFactory.CreateMock(3, TENANT_ID);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, mockClaimsProvider);

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

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

        var result = await getEntriesByPeriodQuery
            .GetByPeriodAsync<TaskEntry>(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.DoesNotContain(result, x => x.Id == taskEntry.Id);
    }

    [Fact]
    public async Task GetEntriesByPeriodAsync_ShouldReturnEntryWhenEntryEndsExactlyOnPeriodStart()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var sickLeaveEntry = new SickLeaveEntry
        {
            Id = 1,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2026, 7, 13, 0, 0, 0),
            EndTime = new DateTime(2026, 7, 20, 0, 0, 0),
            Type = EntryType.SickLeave
        };

        await context.AddEntityAndSaveAsync(sickLeaveEntry);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

        var startDate = new DateOnly(2026, 7, 20);
        var endDate = new DateOnly(2026, 7, 26);

        var result = await getEntriesByPeriodQuery.GetByPeriodAsync<TrackedEntryBase>(startDate, endDate);

        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Id == sickLeaveEntry.Id);
    }

    [Fact]
    public async Task GetEntriesByPeriodAsync_ShouldReturnEntryWhenEntryStartsExactlyOnPeriodEnd()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var sickLeaveEntry = new SickLeaveEntry
        {
            Id = 1,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2026, 7, 13, 0, 0, 0),
            EndTime = new DateTime(2026, 7, 20, 0, 0, 0),
            Type = EntryType.SickLeave
        };

        await context.AddEntityAndSaveAsync(sickLeaveEntry);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

        var startDate = new DateOnly(2026, 7, 10);
        var endDate = new DateOnly(2026, 7, 13);

        var result = await getEntriesByPeriodQuery.GetByPeriodAsync<TrackedEntryBase>(startDate, endDate);

        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Id == sickLeaveEntry.Id);
    }

    [Fact]
    public async Task GetEntriesByPeriodAsync_ShouldNotReturnEntryWhenEntryEndsBeforePeriodStart()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var sickLeaveEntry = new SickLeaveEntry
        {
            Id = 1,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2026, 7, 10, 0, 0, 0),
            EndTime = new DateTime(2026, 7, 12, 0, 0, 0),
            Type = EntryType.SickLeave
        };

        await context.AddEntityAndSaveAsync(sickLeaveEntry);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

        var startDate = new DateOnly(2026, 7, 13);
        var endDate = new DateOnly(2026, 7, 19);

        var result = await getEntriesByPeriodQuery.GetByPeriodAsync<TrackedEntryBase>(startDate, endDate);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEntriesByPeriodAsync_ShouldNotReturnEntryWhenEntryStartsAfterPeriodEnd()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var sickLeaveEntry = new SickLeaveEntry
        {
            Id = 1,
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2026, 7, 20, 0, 0, 0),
            EndTime = new DateTime(2026, 7, 26, 0, 0, 0),
            Type = EntryType.SickLeave
        };

        await context.AddEntityAndSaveAsync(sickLeaveEntry);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

        var startDate = new DateOnly(2026, 7, 13);
        var endDate = new DateOnly(2026, 7, 19);

        var result = await getEntriesByPeriodQuery.GetByPeriodAsync<TrackedEntryBase>(startDate, endDate);

        Assert.Empty(result);
    }
}
