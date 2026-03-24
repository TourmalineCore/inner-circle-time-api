using Core.Entities;
using Xunit;

namespace Core;

public class CalculateTotalTrackedTaskHoursTest
{
    [Fact]
    public void CalculateTotalTrackedTaskHours_ShouldReturnCorrectEmployeeTrackedTaskHours()
    {
        var employeesTaskEntries = new List<TaskEntry> {
            new TaskEntry
            {
                Id = 1,
                EmployeeId = 1,
                Duration = TimeSpan.Parse("02:00:00")
            },
            new TaskEntry
            {
                Id = 2,
                EmployeeId = 1,
                Duration = TimeSpan.Parse("03:00:00")
            },
            new TaskEntry {
                Id = 3,
                EmployeeId = 2,
                Duration = TimeSpan.Parse("02:59:59")
            }
        };

        var calculatedEmployeesTaskEntries = CalculateTotalTrackedTaskHours.Calculate(employeesTaskEntries);

        var firstEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 1);

        Assert.Equal(5, firstEmployee.TrackedHours);
    }
}
