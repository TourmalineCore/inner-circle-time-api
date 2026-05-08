using Core.Entities;
using Xunit;

namespace Core;

[UnitTest]
public class TotalTrackedTaskMinutesCalculatorTests
{
    [Fact]
    public void TotalTrackedTaskMinutesCalculatorWithoutEmployeesTaskEntries_ShouldReturnEmptyList()
    {
        var employeesTaskEntries = new List<TaskEntry> { };

        var calculatedEmployeesTaskEntries = TotalTrackedTaskMinutesCalculator.Calculate(employeesTaskEntries);

        Assert.Empty(calculatedEmployeesTaskEntries);
    }

    [Fact]
    public void TotalTrackedTaskMinutesCalculatorWithOneEmployeeWithOneTaskEntry_ShouldReturnCorrectEmployeeTrackedTaskMinutes()
    {
        var employeesTaskEntries = new List<TaskEntry> {
            new TaskEntry
            {
                Id = 1,
                EmployeeId = 1,
                StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 11, 35, 0),
            },
        };

        var calculatedEmployeesTaskEntries = TotalTrackedTaskMinutesCalculator.Calculate(employeesTaskEntries);

        var firstEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 1);

        Assert.Equal(1, firstEmployee.EmployeeId);
        Assert.Equal(155, firstEmployee.TrackedMinutes);
    }

    [Fact]
    public void TotalTrackedTaskMinutesCalculatorWithOneEmployeeWithTwoTaskEntries_ShouldReturnCorrectEmployeeTrackedTaskMinutes()
    {
        var employeesTaskEntries = new List<TaskEntry> {
            new TaskEntry
            {
                Id = 1,
                EmployeeId = 1,
                StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 10, 05, 0),
            },
            new TaskEntry
            {
                Id = 2,
                EmployeeId = 1,
                StartTime = new DateTime(2025, 11, 24, 10, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 12, 20, 0),
            },
        };

        var calculatedEmployeesTaskEntries = TotalTrackedTaskMinutesCalculator.Calculate(employeesTaskEntries);

        var firstEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 1);

        Assert.Equal(1, firstEmployee.EmployeeId);
        Assert.Equal(205, firstEmployee.TrackedMinutes);
    }

    [Fact]
    public void TotalTrackedTaskMinutesCalculatorWithTwoEmployees_ShouldReturnCorrectEmployeesTrackedTaskMinutes()
    {
        var employeesTaskEntries = new List<TaskEntry> {
            new TaskEntry
            {
                Id = 1,
                EmployeeId = 1,
                StartTime = new DateTime(2025, 11, 24, 9, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 11, 45, 0),
            },
            new TaskEntry {
                Id = 3,
                EmployeeId = 2,
                StartTime = new DateTime(2025, 11, 24, 13, 0, 0),
                EndTime = new DateTime(2025, 11, 24, 17, 45, 0),
            }
        };

        var calculatedEmployeesTaskEntries = TotalTrackedTaskMinutesCalculator.Calculate(employeesTaskEntries);

        var firstEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 1);
        var secondEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 2);

        Assert.Equal(1, firstEmployee.EmployeeId);
        Assert.Equal(165, firstEmployee.TrackedMinutes);

        Assert.Equal(2, secondEmployee.EmployeeId);
        Assert.Equal(285, secondEmployee.TrackedMinutes);
    }
}
