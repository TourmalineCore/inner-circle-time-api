using Core.Entities;

namespace Core
{
    public class CalculateTotalTrackedTaskHours
    {
        public static List<EmployeeTrackedTaskHours> Calculate(List<TaskEntry> employeeTaskEntries)
        {
            var employeeTrackedTaskHours = employeeTaskEntries.Select(
                x => new EmployeeTrackedTaskHours
                {
                    EmployeeId = x.EmployeeId,
                    TrackedHours = (x.EndTime - x.StartTime).TotalHours
                })
                .GroupBy(x => x.EmployeeId)
                .Select(v => new EmployeeTrackedTaskHours
                {
                    EmployeeId = v.Key,
                    TrackedHours = v.Sum(x => x.TrackedHours)
                })
                .ToList();

            return employeeTrackedTaskHours;
        }
    }
}
