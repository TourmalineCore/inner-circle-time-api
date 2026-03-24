using Core.Entities;

namespace Core
{
    public class CalculateTotalTrackedTaskHours
    {
        public static List<EmployeeTrackedTaskHours> Calculate(List<TaskEntry> employeeTaskEntries)
        {
            var employeeTrackedTaskHours = employeeTaskEntries
                .GroupBy(x => x.EmployeeId)
                .Select(x => new EmployeeTrackedTaskHours
                {
                    EmployeeId = x.Key,
                    TrackedHours = x.Sum(x => x.Duration.TotalHours)
                })
                .ToList();

            return employeeTrackedTaskHours;
        }
    }
}
