namespace Application.Features.Reporting.GetAllEmployees;

public class GetAllEmployeesHandler
{
    public GetAllEmployeesHandler()
    {
    }

    public async Task<GetAllEmployeesResponse> HandleAsync()
    {

        return new GetAllEmployeesResponse
        {
            Employees = new List<EmployeeDto> { }
        };
    }
}
