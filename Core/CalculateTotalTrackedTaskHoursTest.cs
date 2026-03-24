using Core.Entities;
using Xunit;

namespace Core;

public class HolderEmployeeMapperTests
{
    [Fact]
    public void CalculateTotalTrackedTaskHours_HappyPath()
    {
        var employeesTaskEntries = new List<TaskEntry> {
            new TaskEntry
            {
                Id = 1,
                EmployeeId = 1,
                StartTime = DateTime.Parse("2026-03-01T00:00:00"),
                EndTime = DateTime.Parse("2026-03-01T02:00:00"),
            },
            new TaskEntry
            {
                Id = 2,
                EmployeeId = 1,
                StartTime = DateTime.Parse("2026-03-19T14:00:00"),
                EndTime = DateTime.Parse("2026-03-19T17:00:00")
            },
            new TaskEntry {
                Id = 3,
                EmployeeId = 2,
                StartTime = DateTime.Parse("2026-03-31T21:00:00"),
                EndTime = DateTime.Parse("2026-03-31T23:59:59")
            }
        };

        var calculatedEmployeesTaskEntries = CalculateTotalTrackedTaskHours.Calculate(employeesTaskEntries);

        var firstEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 1);

        Assert.Equal(5, firstEmployee.TrackedHours);
    }
}
