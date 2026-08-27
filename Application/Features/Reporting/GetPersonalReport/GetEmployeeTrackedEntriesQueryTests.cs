using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.Reporting.GetPersonalReport;

[IntegrationTest]
public class GetEmployeeTrackedEntriesQueryTests : IntegrationTestBase
{
    [Fact]
    public async Task GetTrackedEntriesWithNonExistentEmployeeId__ShouldReturnEmptyTrackedEntriesList()
    {
        var context = CreateTenantDbContext();

        var getEmployeeTrackedEntriesQuery = new GetEmployeeTrackedEntriesQuery(context);

        var taskEntry = new TaskEntry
        {
            Id = 11,
            TenantId = TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(taskEntry);

        var nonExistentEmployeeId = 999;

        var result = await getEmployeeTrackedEntriesQuery
            .GetAsync(
                nonExistentEmployeeId,
                new DateOnly(2025, 11, 01),
                new DateOnly(2025, 11, 30)
            );

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTrackedEntriesForThePeriodWithoutEntries__ShouldReturnEmptyTrackedEntriesList()
    {
        var context = CreateTenantDbContext();

        var getEmployeeTrackedEntriesQuery = new GetEmployeeTrackedEntriesQuery(context);

        var taskEntry = new TaskEntry
        {
            Id = 11,
            TenantId = TENANT_ID,
            EmployeeId = 1,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(taskEntry);

        var result = await getEmployeeTrackedEntriesQuery
            .GetAsync(
                taskEntry.EmployeeId,
                new DateOnly(2025, 11, 20),
                new DateOnly(2025, 11, 21)
            );

        Assert.Empty(result);
    }
}
