namespace Application.Features.Reporting.GetAllEmployees;

public class GetAllEmployeesResponse
{
    public required List<EmployeeDto> Employees { get; set; }
}

public class EmployeeDto
{
    public required long Id { get; set; }

    public required string FullName { get; set; }
}
