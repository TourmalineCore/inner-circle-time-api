using Core.Entities;
using Moq;
using Xunit;

namespace Application.Queries;

[Trait("Type", "Unit")]
public class GetWorkEntriesByPeriodQueryTests
{
    private const long EMPLOYEE_ID = 1;

    [Fact]
    public async Task GetWorkEntriesByPeriodAsync_ShouldReturnWorkEntriesByPeriodFromDbSet()
    {
        var context = TenantAppDbContextExtensionsTestsRelated.CreateInMemoryTenantContextForTests();
        var mockClaimsProvider = new Mock<IClaimsProvider>();

        mockClaimsProvider
            .Setup(cp => cp.EmployeeId)
            .Returns(EMPLOYEE_ID);

        var getWorkEntriesByPeriodQuery = new GetWorkEntriesByPeriodQuery(context, mockClaimsProvider.Object);

        var workEntry1 = new WorkEntry
        {
            Id = 11,
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 1",
            StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 24, 10, 0, 0),
            TaskId = "#2231",
            Type = EventType.Task
        };

        var workEntry2 = new WorkEntry
        {
            Id = 12,
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 2",
            StartTime = new DateTime(2025, 11, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 11, 27, 10, 0, 0),
            TaskId = "#2232",
            Type = EventType.Task
        };

        var workEntry3 = new WorkEntry
        {
            Id = 13,
            EmployeeId = EMPLOYEE_ID,
            Title = "Task 3",
            StartTime = new DateTime(2025, 10, 27, 9, 0, 0),
            EndTime = new DateTime(2025, 10, 27, 10, 0, 0),
            TaskId = "#2233",
            Type = EventType.Task
        };

        await context.AddEntityAndSaveAsync(workEntry1);
        await context.AddEntityAndSaveAsync(workEntry2);
        await context.AddEntityAndSaveAsync(workEntry3);

        var result = await getWorkEntriesByPeriodQuery
            .GetByPeriodAsync(
                new DateTime(2025, 11, 24, 0, 0, 0),
                new DateTime(2025, 11, 27, 23, 59, 59)
            );

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, w => w.Title == workEntry1.Title && w.TaskId == workEntry1.TaskId);
        Assert.Contains(result, w => w.Title == workEntry2.Title && w.TaskId == workEntry2.TaskId);
        Assert.DoesNotContain(result, w => w.Title == workEntry3.Title && w.TaskId == workEntry2.TaskId);
    }
}
