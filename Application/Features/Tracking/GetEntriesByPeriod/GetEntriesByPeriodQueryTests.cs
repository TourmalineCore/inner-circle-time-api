using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.Tracking.GetEntriesByPeriod;

[IntegrationTest]
public class GetEntriesByPeriodQueryTests : IntegrationTestBase
{
    private IClaimsProvider _mockClaimsProvider = MockClaimsProviderFactory.CreateMock(EMPLOYEE_ID, TENANT_ID);

    [Fact]
    public async Task GetEntriesByPeriodAsync_ShouldReturnEntriesByPeriodFromDbSet()
    {
        var context = CreateTenantDbContext();

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

        var taskEntry1 = new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        var taskEntry2 = new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 27, 10, 0, 0),
        };

        var taskEntry3 = new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 10, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 10, 27, 10, 0, 0),
        };

        await AddEntityAndSaveAsync(context, taskEntry1);
        await AddEntityAndSaveAsync(context, taskEntry2);
        await AddEntityAndSaveAsync(context, taskEntry3);

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
        var context = CreateTenantDbContext();

        var taskEntry = new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await AddEntityAndSaveAsync(context, taskEntry);

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
        var context = CreateTenantDbContext();

        var taskEntry = new TaskEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = 3,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await AddEntityAndSaveAsync(context, taskEntry);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

        var result = await getEntriesByPeriodQuery
            .GetByPeriodAsync<TaskEntry>(
                new DateOnly(2025, 11, 24),
                new DateOnly(2025, 11, 27)
            );

        Assert.DoesNotContain(result, x => x.Id == taskEntry.Id);
    }


    public static IEnumerable<object[]> SickLeaveEntryOverlapsPeriodTestData()
    {
        return new List<object[]>
        {
            // sick leave starts before the period and ends on the first day of the period
            new object[] {
                new DateTime(2026, 7, 13, 0, 0, 0), // sickLeaveStartTime
                new DateTime(2026, 7, 20, 0, 0, 0), // sickLeaveEndTime
                new DateOnly(2026, 7, 20),          // periodStartDate
                new DateOnly(2026, 7, 26),          // periodEndDate
                true                                // shouldEntryBeReturned
            },
            // sick leave starts on the last day of the period and lasts longer than the period
            new object[] {
                new DateTime(2026, 7, 13, 0, 0, 0),
                new DateTime(2026, 7, 20, 0, 0, 0),
                new DateOnly(2026, 7, 10),
                new DateOnly(2026, 7, 13),
                true
            },
            // sick leave starts and ends before the period
            new object[] {
                new DateTime(2026, 7, 10, 0, 0, 0),
                new DateTime(2026, 7, 12, 0, 0, 0),
                new DateOnly(2026, 7, 13),
                new DateOnly(2026, 7, 19),
                false
            },
            // sick leave starts and ends after the period
            new object[] {
                new DateTime(2026, 7, 20, 0, 0, 0),
                new DateTime(2026, 7, 26, 0, 0, 0),
                new DateOnly(2026, 7, 13),
                new DateOnly(2026, 7, 19),
                false
            },
            // sick leave starts before the period and ends after the period
            new object[] {
                new DateTime(2026, 7, 20, 0, 0, 0),
                new DateTime(2026, 7, 26, 0, 0, 0),
                new DateOnly(2026, 7, 21),
                new DateOnly(2026, 7, 25),
                true
            },
            // sick leave is within the period
            new object[] {
                new DateTime(2026, 7, 21, 0, 0, 0),
                new DateTime(2026, 7, 24, 0, 0, 0),
                new DateOnly(2026, 7, 20),
                new DateOnly(2026, 7, 25),
                true
            }
        };
    }

    [Theory]
    [MemberData(nameof(SickLeaveEntryOverlapsPeriodTestData))]
    public async Task GetEntriesByPeriodAsync_ShouldReturnEntryWhenItOverlapsPeriod(
        DateTime sickLeaveStartTime,
        DateTime sickLeaveEndTime,
        DateOnly periodStartDate,
        DateOnly periodEndDate,
        bool shouldEntryBeReturned
    )
    {
        var context = CreateTenantDbContext();

        var sickLeaveEntry = new SickLeaveEntry
        {
            EmployeeId = EMPLOYEE_ID,
            TenantId = TENANT_ID,
            StartTime = sickLeaveStartTime,
            EndTime = sickLeaveEndTime,
            Type = EntryType.SickLeave
        };

        await AddEntityAndSaveAsync(context, sickLeaveEntry);

        var getEntriesByPeriodQuery = new GetEntriesByPeriodQuery(context, _mockClaimsProvider);

        var startDate = periodStartDate;
        var endDate = periodEndDate;

        var result = await getEntriesByPeriodQuery.GetByPeriodAsync<TrackedEntryBase>(startDate, endDate);

        Assert.Equal(shouldEntryBeReturned, result.Any(x => x.Id == sickLeaveEntry.Id));
    }
}
