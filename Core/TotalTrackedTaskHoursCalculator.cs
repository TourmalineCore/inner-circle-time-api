using Core.Entities;

namespace Core;

public class EmployeeTrackedTaskHours
{
    public required long EmployeeId { get; set; }

    public required decimal TrackedHours { get; set; }
}

public class TotalTrackedTaskHoursCalculator
{
    public static List<EmployeeTrackedTaskHours> Calculate(List<TaskEntry> employeeTaskEntries)
    {
        var employeeTrackedTaskHours = employeeTaskEntries
            .GroupBy(x => x.EmployeeId)
            .Select(x => new EmployeeTrackedTaskHours
            {
                EmployeeId = x.Key,
                TrackedHours = (decimal)x.Sum(x => x.Duration.TotalHours)
            })
            .ToList();

        return employeeTrackedTaskHours;
    }
}
