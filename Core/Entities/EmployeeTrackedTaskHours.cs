namespace Core.Entities
{
    public class EmployeeTrackedTaskHours
    {
        public required long EmployeeId { get; set; }
        public required double TrackedHours { get; set; }
    }
}
