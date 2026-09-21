namespace Application.Features.Internal.Handlers.GetEmployeesTrackedTaskHours;

public class GetEmployeesTrackedTaskHoursResponse
{
    public required List<EmployeeTrackedTaskHourDto> EmployeesTrackedTaskHours { get; set; }
}

public class EmployeeTrackedTaskHourDto
{
    public required long EmployeeId { get; set; }

    public required decimal TrackedHours { get; set; }
}
