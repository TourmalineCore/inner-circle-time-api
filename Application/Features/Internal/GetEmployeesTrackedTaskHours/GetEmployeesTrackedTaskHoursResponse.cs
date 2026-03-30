namespace Application.Features.Internal.GetEmployeesTrackedTaskHours;

public class GetEmployeesTrackedTaskHoursResponse
{
    public required List<EmployeeTrackedTaskHourDto> EmployeesTrackedTaskHours { get; set; }
}

public class EmployeeTrackedTaskHourDto
{
    public required long EmployeeId { get; set; }

    public required double TrackedHours { get; set; }
}
