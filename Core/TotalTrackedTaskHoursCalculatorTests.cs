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
                StartTime = DateTime.Parse("2026-03-01T00:00:00"),
                EndTime = DateTime.Parse("2026-03-01T02:59:00"),
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
                StartTime = DateTime.Parse("2026-03-01T00:00:00"),
                EndTime = DateTime.Parse("2026-03-01T02:00:00"),
            },
            new TaskEntry
            {
                Id = 2,
                EmployeeId = 1,
                StartTime = DateTime.Parse("2026-03-01T00:00:00"),
                EndTime = DateTime.Parse("2026-03-01T03:00:00"),
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
                StartTime = DateTime.Parse("2026-03-01T00:00:00"),
                EndTime = DateTime.Parse("2026-03-01T02:45:00"),
            },
            new TaskEntry {
                Id = 3,
                EmployeeId = 2,
                StartTime = DateTime.Parse("2026-03-01T00:00:00"),
                EndTime = DateTime.Parse("2026-03-01T04:30:00"),
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
