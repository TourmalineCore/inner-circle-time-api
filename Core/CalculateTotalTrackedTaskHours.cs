using Core.Entities;

namespace Core
{
    public class CalculateTotalTrackedTaskHours
    {
        public static List<EmployeeTrackedTaskHours> Calculate(List<TaskEntry> employeeTaskEntries)
        {
            var employeeTrackedTaskHours = employeeTaskEntries
                .Select(x => new EmployeeTrackedTaskHours
                {
                    EmployeeId = x.EmployeeId,
                    TrackedHours = x.Duration.TotalHours
                })
                .GroupBy(x => x.EmployeeId)
                .Select(y => new EmployeeTrackedTaskHours
                {
                    EmployeeId = y.Key,
                    TrackedHours = y.Sum(x => x.TrackedHours)
                })
                .ToList();

            return employeeTrackedTaskHours;
        }
    }
}
