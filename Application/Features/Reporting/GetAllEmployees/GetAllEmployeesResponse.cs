namespace Application.Features.Reporting.GetAllEmployees;

public class GetAllEmployeesResponse
{
    public required List<EmployeeDto> Employees { get; set; }
}
