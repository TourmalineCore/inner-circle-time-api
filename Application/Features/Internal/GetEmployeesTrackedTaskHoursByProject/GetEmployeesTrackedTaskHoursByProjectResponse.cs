using Core.Entities;

namespace Application.Features.Internal.GetEmployeesTrackedTaskHoursByProject;

public class GetEmployeesTrackedTaskHoursByProjectResponse
{
    public required List<EmployeeTrackedTaskHours> EmployeesTrackedTaskHours { get; set; }
}

