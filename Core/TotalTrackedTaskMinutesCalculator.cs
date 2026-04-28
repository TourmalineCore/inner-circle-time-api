using Core.Entities;

namespace Core;

public class EmployeeTrackedTaskMinutes
{
    public required long EmployeeId { get; set; }

    public required int TrackedMinutes { get; set; }
}

public class TotalTrackedTaskMinutesCalculator
{
    public static List<EmployeeTrackedTaskMinutes> Calculate(List<TaskEntry> employeeTaskEntries)
    {
        var employeeTrackedTaskMinutes = employeeTaskEntries
            .GroupBy(x => x.EmployeeId)
            .Select(x => new EmployeeTrackedTaskMinutes
            {
                EmployeeId = x.Key,
                TrackedMinutes = x.Sum(x => x.StartTime.GetTotalMinutes(x.EndTime))
            })
            .ToList();

        return employeeTrackedTaskMinutes;
    }
}
