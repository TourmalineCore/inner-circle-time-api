using Core;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHours;

[UnitTest]
public class GetEmployeesTrackedTaskHoursHandlerTests
{
    [Fact]
    public async Task GetEmployeesTrackedTaskHoursHandler_ShouldReturnCorrectResultWithoutRoundingTrackedHours()
    {
        var taskEntries = new List<TaskEntry>{
            new TaskEntry
            {
                Id = 11,
                EmployeeId = 1,
                TenantId = 1,
                ProjectId = 1,
                StartTime = new DateTime(2025, 11, 24, 10, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 10, 30, 0),
            },
             new TaskEntry
            {
                Id = 11,
                EmployeeId = 1,
                TenantId = 1,
                ProjectId = 1,
                StartTime = new DateTime(2025, 11, 24, 11, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 11, 40, 0),
            },
        };

        var getTaskEntriesQueryMock = new Mock<IGetTaskEntriesQuery>();

        getTaskEntriesQueryMock
            .Setup(x => x.GetAsync(It.IsAny<long>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(taskEntries);

        var getEmployeesTrackedTaskHoursHandler = new GetEmployeesTrackedTaskHoursHandler(
            getTaskEntriesQueryMock.Object
        );

        var result = await getEmployeesTrackedTaskHoursHandler.HandleAsync(1, new DateOnly(2025, 11, 1), new DateOnly(2025, 11, 28));

        Assert.NotEmpty(result.EmployeesTrackedTaskHours);
        Assert.Equal(1.1666666666666666666666666667m, result.EmployeesTrackedTaskHours[0].TrackedHours);
        Assert.Equal(1, result.EmployeesTrackedTaskHours[0].EmployeeId);
    }
}
