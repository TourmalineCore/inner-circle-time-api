using Core.Entities;
using Xunit;

namespace Core;

[UnitTest]
public class TotalTrackedTaskHoursCalculatorTests
{
    [Fact]
    public void TotalTrackedTaskHoursCalculatorWithoutEmployeesTaskEntries_ShouldReturnEmptyList()
    {
        var employeesTaskEntries = new List<TaskEntry> { };

        var calculatedEmployeesTaskEntries = TotalTrackedTaskHoursCalculator.Calculate(employeesTaskEntries);

        Assert.Empty(calculatedEmployeesTaskEntries);
    }

    [Fact]
    public void TotalTrackedTaskHoursCalculatorWithOneEmployeeWithOneTaskEntry_ShouldReturnCorrectEmployeeTrackedTaskHours()
    {
        var employeesTaskEntries = new List<TaskEntry> {
            new TaskEntry
            {
                Id = 1,
                EmployeeId = 1,
                Duration = TimeSpan.Parse("02:59:00")
            },
        };

        var calculatedEmployeesTaskEntries = TotalTrackedTaskHoursCalculator.Calculate(employeesTaskEntries);

        var firstEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 1);

        Assert.Equal(1, firstEmployee.EmployeeId);
        Assert.Equal(2.9833333333333334, firstEmployee.TrackedHours);
    }

    [Fact]
    public void TotalTrackedTaskHoursCalculatorWithOneEmployeeWithTwoTaskEntries_ShouldReturnCorrectEmployeeTrackedTaskHours()
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
        };

        var calculatedEmployeesTaskEntries = TotalTrackedTaskHoursCalculator.Calculate(employeesTaskEntries);

        var firstEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 1);

        Assert.Equal(1, firstEmployee.EmployeeId);
        Assert.Equal(5, firstEmployee.TrackedHours);
    }

    [Fact]
    public void TotalTrackedTaskHoursCalculatorWithTwoEmployees_ShouldReturnCorrectEmployeesTrackedTaskHours()
    {
        var employeesTaskEntries = new List<TaskEntry> {
            new TaskEntry
            {
                Id = 1,
                EmployeeId = 1,
                Duration = TimeSpan.Parse("02:45:00")
            },
            new TaskEntry {
                Id = 3,
                EmployeeId = 2,
                Duration = TimeSpan.Parse("04:30:00")
            }
        };

        var calculatedEmployeesTaskEntries = TotalTrackedTaskHoursCalculator.Calculate(employeesTaskEntries);

        var firstEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 1);
        var secondEmployee = calculatedEmployeesTaskEntries.Single(x => x.EmployeeId == 2);

        Assert.Equal(1, firstEmployee.EmployeeId);
        Assert.Equal(2.75, firstEmployee.TrackedHours);

        Assert.Equal(2, secondEmployee.EmployeeId);
        Assert.Equal(4.5, secondEmployee.TrackedHours);
    }
}
