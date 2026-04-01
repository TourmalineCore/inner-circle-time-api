using Core;
using Core.Entities;
using Xunit;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHours;

[UnitTest]
public class GetTaskEntriesQueryTests
{
    private const long TENANT_ID = 777;

    [Fact]
    public async Task GetTaskEntriesWithNonExistentProjectId__ShouldReturnEmptyTaskEntriesList()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var getTaskEntriesQuery = new GetTaskEntriesQuery(context);

        var taskEntry = new TaskEntry
        {
            Id = 11,
            TenantId = TENANT_ID,
            ProjectId = 1,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(taskEntry);

        var nonExistentProjectId = 999;

        var result = await getTaskEntriesQuery
            .GetAsync(
                nonExistentProjectId,
                new DateOnly(2025, 11, 01),
                new DateOnly(2025, 11, 30)
            );

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTaskEntriesForThePeriodWithoutEntries__ShouldReturnEmptyTaskEntriesList()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests(TENANT_ID);

        var getTaskEntriesQuery = new GetTaskEntriesQuery(context);

        var taskEntry = new TaskEntry
        {
            Id = 11,
            TenantId = TENANT_ID,
            ProjectId = 1,
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
        };

        await context.AddEntityAndSaveAsync(taskEntry);

        var result = await getTaskEntriesQuery
            .GetAsync(
                taskEntry.ProjectId,
                new DateOnly(2025, 11, 20),
                new DateOnly(2025, 11, 21)
            );

        Assert.Empty(result);
    }
}
