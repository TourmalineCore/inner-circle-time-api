namespace Application.Features.Reporting.Handlers.GetAllEmployees;

public class GetAllEmployeesResponse
{
    public required List<EmployeeDto> Employees { get; set; }
}
