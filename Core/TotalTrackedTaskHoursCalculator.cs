using Core.Entities;

namespace Core;

public class EmployeeTrackedTaskHours
{
    public required long EmployeeId { get; set; }

    public required double TrackedHours { get; set; }
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
                TrackedHours = x.Sum(x => (x.EndTime - x.StartTime).TotalHours)
            })
            .ToList();

        return employeeTrackedTaskHours;
    }
}
