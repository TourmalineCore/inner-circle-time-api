using Core.Entities;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHoursByProject;

public class GetEmployeesTrackedTaskHoursByProjectResponse
{
    public required List<EmployeeTrackedTaskHourDto> EmployeesTrackedTaskHours { get; set; }
}

public class EmployeeTrackedTaskHourDto
{
    public required long EmployeeId { get; set; }

    public required double TrackedHours { get; set; }
}

